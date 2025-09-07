using Cssao.Domain.Services;

namespace Cssao.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CurrentUserService> _logger;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor, ILogger<CurrentUserService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirst("uid")?.Value;

        public string? UserName =>
            _httpContextAccessor.HttpContext?.User?.Identity?.Name
            ?? _httpContextAccessor.HttpContext?.User?.FindFirst("email")?.Value
            ?? "system";

        public bool IsAuthenticated
        {
            get
            {
                var httpContext = _httpContextAccessor.HttpContext;

                if (httpContext == null)
                {
                    _logger.LogWarning("HttpContext 为 null，无法获取用户身份");
                    return false;
                }

                var user = httpContext.User;
                if (user == null || user.Identity == null)
                {
                    _logger.LogDebug("User 或 Identity 为 null");
                    return false;
                }

                var isAuthenticated = user.Identity.IsAuthenticated;
                _logger.LogInformation("用户认证状态: {IsAuthenticated}, 用户名: {UserName}",
                    isAuthenticated, user.Identity.Name);

                return isAuthenticated;
            }
        }
    }
}
