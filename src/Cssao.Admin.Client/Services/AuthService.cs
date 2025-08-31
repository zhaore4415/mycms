using Cssao.Shared.Models;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Cssao.Admin.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;

        public string? Username { get; private set; }

        public bool IsAuthenticated => !string.IsNullOrEmpty(Username);

        public AuthService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
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
            await _js.InvokeVoidAsync("localStorage.removeItem", "authToken");
            await _js.InvokeVoidAsync("localStorage.removeItem", "username");
            _http.DefaultRequestHeaders.Authorization = null;
            Username = null;
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
