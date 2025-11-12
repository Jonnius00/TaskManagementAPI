namespace TaskManagementAPI.DTOs
{
    public class RegisterDto
    {
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    public class LoginDto
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    public class AuthResultDto
    {
        public string Token { get; set; } = default!;
        public int UserId { get; set; }
        public string Username { get; set; } = default!;
    }
}
