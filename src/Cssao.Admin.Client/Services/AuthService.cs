using Cssao.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Cssao.Admin.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;
        private readonly NavigationManager _navigation;
        private readonly PersistentAuthenticationStateProvider  _authStateProvider;

        public AuthService(
            HttpClient http,
            IJSRuntime js,
            NavigationManager navigation,
            PersistentAuthenticationStateProvider  authStateProvider)
        {
            _http = http;
            _js = js;
            _navigation = navigation;
            _authStateProvider = authStateProvider;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var loginDto = new LoginRequestDto
            {
                Username = username,
                Password = password
            };

            try
            {
                var response = await _http.PostAsJsonAsync("api/admin/auth/login", loginDto);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                    if (result != null)
                    {
                        // 保存 Token
                        await _js.InvokeVoidAsync("localStorage.setItem", "authToken", result.Token);

                        // 设置 HttpClient 认证头（可选，推荐用 AuthorizationMessageHandler）
                        _http.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);

                        // ✅ 通知授权状态变化
                        _authStateProvider.NotifyUserLoggedIn(result.Token);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"登录失败: {ex.Message}");
            }

            return false;
        }

        public async Task LogoutAsync()
        {
            try
            {
                await _http.PostAsync("api/admin/auth/logout", null);
            }
            catch { /* 忽略 */ }

            try
            {
                await _js.InvokeVoidAsync("localStorage.removeItem", "authToken");
            }
            catch (JSException jsEx)
            {
                Console.WriteLine($"JS错误: {jsEx.Message}");
            }

            // 清除认证头
            _http.DefaultRequestHeaders.Authorization = null;

            // 通知登出
            _authStateProvider.NotifyUserLoggedOut();

            // 跳转
            _navigation.NavigateTo("/login");
        }
    }
}