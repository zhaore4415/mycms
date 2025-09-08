using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components;

namespace Cssao.Admin.Client.Handlers
{
    public class AuthorizationMessageHandler : DelegatingHandler
    {
        private readonly IAccessTokenProvider _provider;
        private readonly NavigationManager _navigation;

        public AuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigation)
        {
            _provider = provider;
            _navigation = navigation;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // 1. 获取当前用户的访问令牌
            var tokenResult = await _provider.RequestAccessToken();

            if (tokenResult.TryGetToken(out var token))
            {
                // 2. 添加到请求头
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Value);
            }

            // 3. 发送请求
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
