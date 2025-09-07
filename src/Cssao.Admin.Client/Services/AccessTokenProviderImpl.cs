// Services/AccessTokenProviderImpl.cs
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.JSInterop;

namespace Cssao.Admin.Client.Services
{
    public class AccessTokenProviderImpl : IAccessTokenProvider
    {
        private readonly PersistentAuthenticationStateProvider _authStateProvider;
        private readonly IJSRuntime _jsRuntime;

        public AccessTokenProviderImpl(
            PersistentAuthenticationStateProvider authStateProvider,
            IJSRuntime jsRuntime)
        {
            _authStateProvider = authStateProvider;
            _jsRuntime = jsRuntime;
        }

        public async ValueTask<AccessTokenResult> RequestAccessToken()
        {
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

            if (!string.IsNullOrEmpty(token))
            {
                return new AccessTokenResult(
                     AccessTokenResultStatus.Success,
                     new AccessToken
                     {
                         Value = token,
                         Expires = ParseExpiryFromJwt(token) // 推荐：从 JWT 解析真实过期时间
                     },
                     interactiveRequestUrl: null,
                     interactiveRequest: null
                 );
            }

            return new AccessTokenResult(
                  AccessTokenResultStatus.RequiresRedirect,
                  token: null,
                  interactiveRequestUrl: "/login", // 或你的登录页
                  interactiveRequest: null
              );
        }

        public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
            => RequestAccessToken();

        // ✅ 添加这个方法：从 JWT 的 payload 中解析 exp 字段
        private DateTimeOffset ParseExpiryFromJwt(string token)
        {
            try
            {
                // JWT 格式: Header.Payload.Signature
                var parts = token.Split('.');
                if (parts.Length != 3) return DateTimeOffset.Now.AddHours(1); // 默认 1 小时

                var payload = parts[1];
                var jsonBytes = ParseBase64WithoutPadding(payload);
                using var jsonDocument = System.Text.Json.JsonDocument.Parse(jsonBytes);

                if (jsonDocument.RootElement.TryGetProperty("exp", out var expClaim))
                {
                    var exp = expClaim.GetRawText(); // 获取原始字符串
                    if (long.TryParse(exp, out long expSeconds))
                    {
                        return DateTimeOffset.FromUnixTimeSeconds(expSeconds);
                    }
                }
            }
            catch
            {
                // 解析失败，返回一个保守的过期时间
            }

            // 默认返回 1 小时后过期
            return DateTimeOffset.Now.AddHours(1);
        }

        // ✅ Base64 解码辅助方法（处理缺少 padding 的情况）
        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}