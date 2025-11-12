using System.Security.Claims;

namespace TaskManagementAPI.Services
{
    public interface IUserContext
    {
        int UserId { get; }
        string? Username { get; }
        bool IsAuthenticated { get; }
    }

    public class HttpUserContext : IUserContext
    {
        private readonly IHttpContextAccessor _http;

        public HttpUserContext(IHttpContextAccessor http) => _http = http;

        public bool IsAuthenticated => _http.HttpContext?.User?.Identity?.IsAuthenticated == true;

        public int UserId
        {
            get
            {
                var sub = _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? _http.HttpContext?.User?.FindFirstValue("sub");
                return int.TryParse(sub, out var id) ? id : 0;
            }
        }

        public string? Username =>
            _http.HttpContext?.User?.FindFirstValue(ClaimTypes.Name)
            ?? _http.HttpContext?.User?.FindFirstValue("unique_name");
    }
}
