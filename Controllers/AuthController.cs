using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly TaskDbContext _db;
        private readonly ITokenService _tokens;

        public AuthController(TaskDbContext db, ITokenService tokens)
        {
            _db = db;
            _tokens = tokens;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResultDto>> Register([FromBody] RegisterDto dto)
        {
            // basic duplicates check
            var exists = await _db.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email);
            if (exists) return Conflict("Username or email already exists.");

            var user = new User
            {
                Username = dto.Username.Trim(),
                Email = dto.Email.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var token = _tokens.CreateToken(user);

            return Ok(new AuthResultDto
            {
                Token = token,
                UserId = user.Id,
                Username = user.Username
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResultDto>> Login([FromBody] LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null) return Unauthorized("Invalid username or password.");

            var ok = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!ok) return Unauthorized("Invalid username or password.");

            var token = _tokens.CreateToken(user);

            return Ok(new AuthResultDto
            {
                Token = token,
                UserId = user.Id,
                Username = user.Username
            });
        }
    }
}
