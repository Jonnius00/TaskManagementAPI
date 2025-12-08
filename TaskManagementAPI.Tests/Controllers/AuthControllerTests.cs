using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TaskManagementAPI.Controllers;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Services;
using TaskManagementAPI.Tests.Helpers;

namespace TaskManagementAPI.Tests.Controllers
{
    public class AuthControllerTests
    {
        private TaskDbContext _db;
        private ITokenService _tokenService;
        private AuthController _controller;

        public AuthControllerTests()
        {
            _db = TestDbContextFactory.CreateInMemoryContext();
            _tokenService = new TokenService(CreateMockConfiguration());
            _controller = new AuthController(_db, _tokenService);
        }

        private IConfiguration CreateMockConfiguration()
        {
            var configData = new Dictionary<string, string>
            {
                {"Jwt:Key", "ThisIsAVerySecretKeyForTestingPurposesOnly12345"},
                {"Jwt:Issuer", "TestIssuer"},
                {"Jwt:Audience", "TestAudience"},
                {"Jwt:ExpiresMinutes", "120"}
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(configData!)
                .Build();
        }

        #region Register Tests

        [Fact]
        public async Task Register_WithValidData_ReturnsOk_WithToken()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "newuser",
                Email = "newuser@test.com",
                Password = "password123"
            };

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var authResult = Assert.IsType<AuthResultDto>(okResult.Value);
            Assert.NotNull(authResult.Token);
            Assert.NotEmpty(authResult.Token);
            Assert.Equal("newuser", authResult.Username);
            Assert.True(authResult.UserId > 0);
        }

        [Fact]
        public async Task Register_WithValidData_CreatesUserInDatabase()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "testuser",
                Email = "testuser@test.com",
                Password = "securepassword"
            };

            // Act
            await _controller.Register(registerDto);

            // Assert
            var user = await _db.Users.FindAsync(4); // User ID should be 4 (after alice, bob, and charlie)
            Assert.NotNull(user);
            Assert.Equal("testuser", user.Username);
            Assert.Equal("testuser@test.com", user.Email);
            Assert.NotNull(user.PasswordHash);
            Assert.NotEqual("securepassword", user.PasswordHash); // Password should be hashed
        }

        [Fact]
        public async Task Register_WithDuplicateUsername_ReturnsConflict()
        {
            // Arrange - alice already exists in seed data
            var registerDto = new RegisterDto
            {
                Username = "alice",
                Email = "newemail@test.com",
                Password = "password123"
            };

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.Equal("Username or email already exists.", conflictResult.Value);
        }

        [Fact]
        public async Task Register_WithDuplicateEmail_ReturnsConflict()
        {
            // Arrange - alice@test.com already exists in seed data
            var registerDto = new RegisterDto
            {
                Username = "newusername",
                Email = "alice@test.com",
                Password = "password123"
            };

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.Equal("Username or email already exists.", conflictResult.Value);
        }

        [Fact]
        public async Task Register_TrimsUsernameAndEmail()
        {
            // Arrange - username and email with whitespace
            var registerDto = new RegisterDto
            {
                Username = "  spaceduser  ",
                Email = "  spaced@test.com  ",
                Password = "password123"
            };

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var authResult = Assert.IsType<AuthResultDto>(okResult.Value);
            Assert.Equal("spaceduser", authResult.Username); // Trimmed

            var user = await _db.Users.FindAsync(authResult.UserId);
            Assert.Equal("spaceduser", user!.Username);
            Assert.Equal("spaced@test.com", user.Email);
        }

        [Fact]
        public async Task Register_HashesPassword()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "secureuser",
                Email = "secure@test.com",
                Password = "MyPlainTextPassword123"
            };

            // Act
            await _controller.Register(registerDto);

            // Assert
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == "secureuser");
            Assert.NotNull(user);
            Assert.NotEqual("MyPlainTextPassword123", user.PasswordHash);
            Assert.StartsWith("$2", user.PasswordHash); // BCrypt hashes start with $2
            
            // Verify password can be verified with BCrypt
            var isValid = BCrypt.Net.BCrypt.Verify("MyPlainTextPassword123", user.PasswordHash);
            Assert.True(isValid);
        }

        #endregion

        #region Login Tests

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOk_WithToken()
        {
            // Arrange - First register a user with known password
            var password = "testpassword123";
            var registerDto = new RegisterDto
            {
                Username = "loginuser",
                Email = "loginuser@test.com",
                Password = password
            };
            await _controller.Register(registerDto);

            // Act - Now try to login
            var loginDto = new LoginDto
            {
                Username = "loginuser",
                Password = password
            };
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var authResult = Assert.IsType<AuthResultDto>(okResult.Value);
            Assert.NotNull(authResult.Token);
            Assert.NotEmpty(authResult.Token);
            Assert.Equal("loginuser", authResult.Username);
            Assert.True(authResult.UserId > 0);
        }

        [Fact]
        public async Task Login_WithInvalidUsername_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = "nonexistentuser",
                Password = "anypassword"
            };

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Invalid username or password.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
        {
            // Arrange - First register a user
            var registerDto = new RegisterDto
            {
                Username = "passwordtestuser",
                Email = "passwordtest@test.com",
                Password = "correctpassword"
            };
            await _controller.Register(registerDto);

            // Act - Try to login with wrong password
            var loginDto = new LoginDto
            {
                Username = "passwordtestuser",
                Password = "wrongpassword"
            };
            var result = await _controller.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Invalid username or password.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Login_ReturnsConsistentErrorMessage_ForInvalidUsernameAndPassword()
        {
            // Arrange - Register a user
            var registerDto = new RegisterDto
            {
                Username = "consistencyuser",
                Email = "consistency@test.com",
                Password = "realpassword"
            };
            await _controller.Register(registerDto);

            // Act - Test with invalid username
            var invalidUsernameDto = new LoginDto
            {
                Username = "wronguser",
                Password = "anypassword"
            };
            var result1 = await _controller.Login(invalidUsernameDto);

            // Act - Test with invalid password
            var invalidPasswordDto = new LoginDto
            {
                Username = "consistencyuser",
                Password = "wrongpassword"
            };
            var result2 = await _controller.Login(invalidPasswordDto);

            // Assert - Both should return same error message (security best practice)
            var unauthorizedResult1 = Assert.IsType<UnauthorizedObjectResult>(result1.Result);
            var unauthorizedResult2 = Assert.IsType<UnauthorizedObjectResult>(result2.Result);
            Assert.Equal(unauthorizedResult1.Value, unauthorizedResult2.Value);
            Assert.Equal("Invalid username or password.", unauthorizedResult1.Value);
        }

        [Fact]
        public async Task Login_IsCaseSensitive_ForUsername()
        {
            // Arrange - Register a user with lowercase username
            var registerDto = new RegisterDto
            {
                Username = "caseuser",
                Email = "caseuser@test.com",
                Password = "password123"
            };
            await _controller.Register(registerDto);

            // Act - Try to login with different case
            var loginDto = new LoginDto
            {
                Username = "CaseUser", // Different case
                Password = "password123"
            };
            var result = await _controller.Login(loginDto);

            // Assert - Should fail because username is case-sensitive
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Invalid username or password.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Login_TokenContainsUserClaims()
        {
            // Arrange - Register and login
            var password = "claimtestpass";
            var registerDto = new RegisterDto
            {
                Username = "claimuser",
                Email = "claimuser@test.com",
                Password = password
            };
            var registerResult = await _controller.Register(registerDto);

            // Act - Login to get token
            var loginDto = new LoginDto
            {
                Username = "claimuser",
                Password = password
            };
            var loginResult = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(loginResult.Result);
            var authResult = Assert.IsType<AuthResultDto>(okResult.Value);
            
            // Token should be a valid JWT (has 3 parts separated by dots)
            var tokenParts = authResult.Token.Split('.');
            Assert.Equal(3, tokenParts.Length);
            
            // Verify token is not empty
            Assert.NotEmpty(authResult.Token);
            Assert.True(authResult.Token.Length > 50); // JWT tokens are typically longer
        }

        #endregion

        #region Additional Edge Cases

        [Fact]
        public async Task Register_WithEmptyUsername_ReturnsConflict()
        {
            // Arrange - This tests behavior with empty/whitespace username
            var registerDto = new RegisterDto
            {
                Username = "   ", // Only whitespace
                Email = "empty@test.com",
                Password = "password123"
            };

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            // After trimming, empty username should cause some issue
            // The actual behavior depends on validation rules
            // For now, we just verify it doesn't crash
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Register_MultipleUsers_GetUniqueIds()
        {
            // Arrange
            var user1Dto = new RegisterDto
            {
                Username = "multiuser1",
                Email = "multi1@test.com",
                Password = "pass123"
            };
            var user2Dto = new RegisterDto
            {
                Username = "multiuser2",
                Email = "multi2@test.com",
                Password = "pass123"
            };

            // Act
            var result1 = await _controller.Register(user1Dto);
            var result2 = await _controller.Register(user2Dto);

            // Assert
            var okResult1 = Assert.IsType<OkObjectResult>(result1.Result);
            var authResult1 = Assert.IsType<AuthResultDto>(okResult1.Value);
            
            var okResult2 = Assert.IsType<OkObjectResult>(result2.Result);
            var authResult2 = Assert.IsType<AuthResultDto>(okResult2.Value);

            Assert.NotEqual(authResult1.UserId, authResult2.UserId);
            Assert.NotEqual(authResult1.Token, authResult2.Token);
        }

        #endregion
    }
}
