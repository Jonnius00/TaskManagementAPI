using Blazored.LocalStorage;
using TaskManagementAPI.Blazor.Models;

namespace TaskManagementAPI.Blazor.Services
{
    /// <summary>
    /// Service for handling user authentication and JWT token management
    /// </summary>
    public class AuthenticationService
    {
        private readonly IApiClient _apiClient;
        private readonly ILocalStorageService _localStorage;
        private readonly ILogger<AuthenticationService> _logger;

        // Notify other components when auth state changes
        public event Action? OnAuthStateChanged;

        public AuthenticationService(
            IApiClient apiClient,
            ILocalStorageService localStorage,
            ILogger<AuthenticationService> logger)
        {
            _apiClient = apiClient;
            _localStorage = localStorage;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user - Returns bool for simple UI handling
        /// </summary>
        public async Task<bool> RegisterAsync(string username, string email, string password)
        {
            var result = await RegisterAsyncDetailed(username, email, password);
            return result.Success;
        }

        /// <summary>
        /// Register a new user - Returns detailed AuthResult
        /// </summary>
        public async Task<AuthResult> RegisterAsyncDetailed(string username, string email, string password)
        {
            try
            {
                _logger.LogInformation($"Attempting to register user: {username}");

                var registerRequest = new RegisterRequest
                {
                    Username = username,
                    Email = email,
                    Password = password
                };

                var response = await _apiClient.PostAsync<AuthResultDto>(
                    "/api/auth/register",
                    registerRequest);

                // Save token (ApiClient now handles localStorage)
                await _apiClient.SetTokenAsync(response.Token);

                _logger.LogInformation($"User {username} registered successfully");
                OnAuthStateChanged?.Invoke();

                return new AuthResult 
                { 
                    Success = true,
                    Message = "Registration successful",
                    UserId = response.UserId,
                    Username = response.Username,
                    Token = response.Token
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Registration failed: {ex.Message}");
                return new AuthResult 
                { 
                    Success = false,
                    Message = $"Registration failed: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Log in an existing user - Returns bool for simple UI handling
        /// </summary>
        public async Task<bool> LoginAsync(string username, string password)
        {
            var result = await LoginAsyncDetailed(username, password);
            return result.Success;
        }

        /// <summary>
        /// Log in an existing user - Returns detailed AuthResult
        /// </summary>
        public async Task<AuthResult> LoginAsyncDetailed(string username, string password)
        {
            try
            {
                _logger.LogInformation($"Attempting to login user: {username}");

                var loginRequest = new LoginRequest
                {
                    Username = username,
                    Password = password
                };

                var response = await _apiClient.PostAsync<AuthResultDto>(
                    "/api/auth/login",
                    loginRequest);

                // Save token (ApiClient now handles localStorage)
                await _apiClient.SetTokenAsync(response.Token);

                _logger.LogInformation($"User {username} logged in successfully");
                OnAuthStateChanged?.Invoke();

                return new AuthResult 
                { 
                    Success = true,
                    Message = "Login successful",
                    UserId = response.UserId,
                    Username = response.Username,
                    Token = response.Token
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login failed: {ex.Message}");
                return new AuthResult 
                { 
                    Success = false,
                    Message = $"Login failed: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Log out the current user
        /// </summary>
        public async Task LogoutAsync()
        {
            try
            {
                _logger.LogInformation("Logging out user");

                // Clear token (ApiClient now handles localStorage)
                await _apiClient.ClearTokenAsync();

                _logger.LogInformation("User logged out successfully");
                OnAuthStateChanged?.Invoke();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Logout failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Initialize authentication on app startup (no longer needed - ApiClient reads from localStorage on each request)
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");

                if (!string.IsNullOrEmpty(token))
                {
                    _logger.LogInformation("Token exists in localStorage");
                    OnAuthStateChanged?.Invoke();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to initialize authentication: {ex.Message}");
            }
        }

        /// <summary>
        /// Check if user is currently authenticated
        /// </summary>
        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            return !string.IsNullOrEmpty(token);
        }

        /// <summary>
        /// Get current user's token
        /// </summary>
        public async Task<string?> GetTokenAsync()
        {
            return await _localStorage.GetItemAsync<string>("authToken");
        }

        /// <summary>
        /// Get current user's username
        /// </summary>
        public async Task<string?> GetCurrentUsernameAsync()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                if (string.IsNullOrEmpty(token))
                    return null;

                // Decode JWT token to get username claim
                var parts = token.Split('.');
                if (parts.Length != 3)
                    return null;

                var payload = parts[1];
                // Add padding if needed
                var padded = payload.Length % 4 == 0 
                    ? payload 
                    : payload + new string('=', 4 - (payload.Length % 4));

                var decoded = System.Text.Encoding.UTF8.GetString(
                    Convert.FromBase64String(padded));

                // Parse JSON to get unique_name claim
                using var doc = System.Text.Json.JsonDocument.Parse(decoded);
                if (doc.RootElement.TryGetProperty("unique_name", out var nameElement))
                {
                    return nameElement.GetString();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get current username: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get current user's ID from JWT token
        /// </summary>
        public async Task<int?> GetCurrentUserIdAsync()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("No auth token found in localStorage");
                    return null;
                }

                // Decode JWT token to get sub claim (user ID)
                var parts = token.Split('.');
                if (parts.Length != 3)
                {
                    _logger.LogError("Invalid JWT token format");
                    return null;
                }

                var payload = parts[1];
                // Add padding if needed
                var padded = payload.Length % 4 == 0 
                    ? payload 
                    : payload + new string('=', 4 - (payload.Length % 4));

                var decoded = System.Text.Encoding.UTF8.GetString(
                    Convert.FromBase64String(padded));

                _logger.LogDebug($"JWT payload: {decoded}");

                // Parse JSON to get sub claim (user ID) - this is the standard JWT claim for subject/user ID
                using var doc = System.Text.Json.JsonDocument.Parse(decoded);
                
                // Try "sub" claim first (JwtRegisteredClaimNames.Sub)
                if (doc.RootElement.TryGetProperty("sub", out var subElement))
                {
                    if (int.TryParse(subElement.GetString(), out var userId))
                    {
                        _logger.LogInformation($"User ID extracted from token: {userId}");
                        return userId;
                    }
                }

                // Fallback: try the full ClaimTypes.NameIdentifier claim name
                if (doc.RootElement.TryGetProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", out var nameIdElement))
                {
                    if (int.TryParse(nameIdElement.GetString(), out var userId))
                    {
                        _logger.LogInformation($"User ID extracted from token (nameidentifier): {userId}");
                        return userId;
                    }
                }

                _logger.LogWarning("Could not find user ID claim in JWT token");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get current user ID: {ex.Message}");
                return null;
            }
        }
    }

    /// <summary>
    /// Request model for login
    /// </summary>
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request model for registration
    /// </summary>
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Result object for authentication operations
    /// </summary>
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
