using Cssao.api.Configuration;
using Cssao.Domain.Entities;
using Cssao.Infrastructure.Data;
using Cssao.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Cssao.api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtSettings _jwtSettings;

        public AuthController(AppDbContext context, IOptions<JwtSettings> jwtSettingsOptions)// ✅ 注入 IOptions<T>
        {
            _context = context;
            _jwtSettings = jwtSettingsOptions.Value; // ✅ 通过 .Value 获取实例
        }
        //[HttpOptions]
        //public IActionResult HandleOptions()
        //{
        //    // CORS 中间件会自动添加响应头
        //    // 我们只需要返回 200 或 204
        //    return Ok(); // 或 NoContent()
        //}
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            // 1. 验证输入
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "用户名和密码不能为空" });
            }

            // 2. 查询数据库
            var user = await _context.AdminUsers
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
            {
                return Unauthorized(new { message = "用户名错误" });
            }

            // 3. 验证密码（假设已用 BCrypt 加密存储）
            #region 写个程序生成加密后的值，或者注册时运行，因为是管理员，暂时在数据库写死的
            //string password = "cssao*888";
            //string hash = BCrypt.Net.BCrypt.HashPassword(password, 12); // 成本因子 12
            //Console.WriteLine(hash); 
            #endregion
            if (string.IsNullOrEmpty(user.PasswordHash) ||
                    !(user.PasswordHash.StartsWith("$2a$") ||
                      user.PasswordHash.StartsWith("$2b$") ||
                      user.PasswordHash.StartsWith("$2y$")))
            {
                // 🔴 可选：记录日志，说明密码格式异常
                Console.WriteLine($"Invalid password hash format for user: {user.Username}");
                return Unauthorized(new { message = "密码错误" });
            }
            try
            {
                bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
                if (!isValid)
                {
                    return Unauthorized(new { message = "密码错误" });
                }
            }
            catch (Exception ex)
            {
                // 🔴 防止因哈希损坏导致 500 错误
                Console.WriteLine($"Password verification error: {ex.Message}");
                return Unauthorized(new { message = "密码错误" });
            }

            // 4. 生成 JWT Token
            var token = GenerateJwtToken(user.Username, user.Role, user.Id);

            // 5. 返回响应（使用共享 DTO）
            var response = new LoginResponseDto
            {
                Token = token,
                Username = user.Username,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            return Ok(response);
        }

        private string GenerateJwtToken(string username, string role, int userId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim("UserId", userId.ToString())
            };

            // ✅ 从配置中读取 Key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "cssao",
                audience: "cssao",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // 1. 获取当前请求的 JWT Token
            var authHeader = Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new { message = "未授权" });
            }

            var token = authHeader["Bearer ".Length..].Trim();

            // 2. 解析 Token 获取过期时间（可选：验证签名）
            JwtSecurityToken jwtToken;
            try
            {
                var handler = new JwtSecurityTokenHandler();
                jwtToken = handler.ReadJwtToken(token);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "无效的 Token 格式" });
            }

            // 3. 检查 Token 是否已过期
            if (jwtToken.ValidTo < DateTime.UtcNow)
            {
                // 已过期，无需加入黑名单
                return Ok(new { message = "登出成功" });
            }

            // 4. 将 Token 加入黑名单
            var blacklistedToken = new BlacklistedToken
            {
                Token = token,
                ExpiredAt = jwtToken.ValidTo // 用于后续清理
            };

            _context.BlacklistedTokens.Add(blacklistedToken);
            await _context.SaveChangesAsync();

            return Ok(new { message = "登出成功" });
        }
    }
}
