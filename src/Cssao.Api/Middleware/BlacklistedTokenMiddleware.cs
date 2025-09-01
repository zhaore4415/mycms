using Cssao.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Threading.Tasks;

namespace Cssao.api.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class BlacklistedTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public BlacklistedTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
        {
            var authHeader = context.Request.Headers.Authorization.ToString();
            if (authHeader.StartsWith("Bearer "))
            {
                var token = authHeader["Bearer ".Length..].Trim();

                // 检查是否在黑名单中
                var isBlacklisted = await dbContext.BlacklistedTokens
                    .AnyAsync(t => t.Token == token);

                if (isBlacklisted)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsJsonAsync(new { message = "Token 已失效，请重新登录" });
                    return;
                }
            }

            await _next(context);
        }
    }

    //// Extension method used to add the middleware to the HTTP request pipeline.
    //public static class BlacklistedTokenMiddlewareExtensions
    //{
    //    public static IApplicationBuilder UseBlacklistedTokenMiddleware(this IApplicationBuilder builder)
    //    {
    //        return builder.UseMiddleware<BlacklistedTokenMiddleware>();
    //    }
    //}
}
