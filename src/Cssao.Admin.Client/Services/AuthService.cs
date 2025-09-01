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
        // private readonly AuthenticationStateProvider _authStateProvider;
        public string? Username { get; private set; }

        public bool IsAuthenticated => !string.IsNullOrEmpty(Username);

        public AuthService(HttpClient http, IJSRuntime js, NavigationManager navigation)
        {
            _http = http;
            _js = js;
            _navigation = navigation;
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
                        // 保存 Token 和用户名
                        await _js.InvokeVoidAsync("localStorage.setItem", "authToken", result.Token);
                        await _js.InvokeVoidAsync("localStorage.setItem", "username", result.Username);

                        _http.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);

                        Username = result.Username;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // 网络错误等
                // 日志记录
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        public async Task LogoutAsync()
        {
            try
            {
                // 可选：通知后端登出（如使用 refresh token 或黑名单）
                var httpResponse = await _http.PostAsync("api/admin/auth/logout", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"登出API调用失败: {ex.Message}");
                // 继续登出流程
            }

            try
            {
                await _js.InvokeVoidAsync("localStorage.removeItem", "authToken");
                await _js.InvokeVoidAsync("localStorage.removeItem", "username");
            }
            catch (JSException jsEx)
            {
                Console.WriteLine($"JavaScript错误: {jsEx.Message}");
            }

            // 清除 HttpClient 认证头
            _http.DefaultRequestHeaders.Authorization = null;

            // 更新本地状态
            Username = null;

            // 可选：通知身份验证状态变化
            // await _authStateProvider.LogoutAsync();

            // 跳转到首页或登录页
            _navigation.NavigateTo("/login", new NavigationOptions { ForceLoad = false });//ForceLoad = true-强制刷新页面
        }

        public async Task<bool> TryRefreshAuthStateAsync()
        {
            var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");
            var username = await _js.InvokeAsync<string>("localStorage.getItem", "username");

            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(username))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                Username = username;
                return true;
            }

            return false;
        }
    }
}
