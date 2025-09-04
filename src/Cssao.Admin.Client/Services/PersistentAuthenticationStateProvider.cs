// Services/PersistentAuthenticationStateProvider.cs
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace Cssao.Admin.Client.Services
{
    public class PersistentAuthenticationStateProvider : AuthenticationStateProvider
    {
        // ✅ 添加这行：默认未认证状态的 Task
        private static readonly Task<AuthenticationState> defaultUnauthenticatedTask =
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        private readonly IJSRuntime _js;
        private Task<AuthenticationState>? _authenticationStateTask;

        public PersistentAuthenticationStateProvider(IJSRuntime js)
        {
            _js = js;
            // ✅ 启动加载，但不 await
            _authenticationStateTask = LoadAuthenticationStateAsync();

            // ✅ 加这一行
            Console.WriteLine("🔧 [Auth] PersistentAuthenticationStateProvider 构造函数被调用！");
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // ✅ 立即返回 Task，避免阻塞
            return _authenticationStateTask ?? defaultUnauthenticatedTask;
        }

        private async Task<AuthenticationState> LoadAuthenticationStateAsync()
        {
            Console.WriteLine("✅ 开始加载认证状态...");

            try
            {
                var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");
                Console.WriteLine($"🔍 获取到 token: {token}");

                if (string.IsNullOrEmpty(token))
                {
                    var state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                    NotifyAuthenticationStateChanged(Task.FromResult(state));
                    Console.WriteLine("👤 用户未登录");
                    return state;
                }

                var claims = ParseClaimsFromJwt(token);
                Console.WriteLine($"🔐 解析出 {claims.Count()} 个 claims");

                var identity = new ClaimsIdentity(claims, "jwt");
                var user = new ClaimsPrincipal(identity);
                var authState = new AuthenticationState(user);

                NotifyAuthenticationStateChanged(Task.FromResult(authState));
                Console.WriteLine("🎉 用户已登录，通知状态更新");

                return authState;
            }
            catch (JSException jsEx)
            {
                Console.WriteLine($"❌ JS Error: {jsEx.Message}");
                var state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                NotifyAuthenticationStateChanged(Task.FromResult(state));
                return state;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ .NET Error: {ex.Message}\n{ex.StackTrace}");
                var state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                NotifyAuthenticationStateChanged(Task.FromResult(state));
                return state;
            }
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            // 你的 JWT 解析逻辑（同之前）
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var jsonDocument = System.Text.Json.JsonDocument.Parse(jsonBytes);
            var claims = new List<Claim>();
            var root = jsonDocument.RootElement;

            if (root.TryGetProperty("sub", out var sub)) claims.Add(new Claim(ClaimTypes.Name, sub.GetString()!));
            if (root.TryGetProperty("name", out var name)) claims.Add(new Claim(ClaimTypes.Name, name.GetString()!));
            if (root.TryGetProperty("role", out var role)) claims.Add(new Claim(ClaimTypes.Role, role.GetString()!));

            jsonDocument.Dispose();
            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        // PersistentAuthenticationStateProvider.cs

        public void NotifyUserLoggedIn(string token)
        {
            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt"); // 认证类型为 "jwt"
            var user = new ClaimsPrincipal(identity);
            var authState = new AuthenticationState(user);

            // ✅ 通知 Blazor 框架：认证状态已改变
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public void NotifyUserLoggedOut()
        {
            // 创建一个空的 ClaimsPrincipal（未登录用户）
            var authState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            // ✅ 通知 Blazor 框架：用户已登出
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }
    }
}