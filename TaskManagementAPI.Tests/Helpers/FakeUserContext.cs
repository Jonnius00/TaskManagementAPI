using TaskManagementAPI.Services;

namespace TaskManagementAPI.Tests.Helpers
{
    public class FakeUserContext : IUserContext
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public bool IsAuthenticated => UserId > 0;

        public FakeUserContext(int userId, string? username = null)
        {
            UserId = userId;
            Username = username ?? $"user{userId}";
        }
    }
}

