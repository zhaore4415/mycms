using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Cssao.Admin.Client;
using Cssao.Admin.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ✅ 1. 必须添加：授权核心服务
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();

//builder.Services.AddScoped(sp => new HttpClient
//{
//    BaseAddress = new Uri("http://localhost:5086/")//  https://localhost:7145/  默认指向 API 同源
//    // 如果 API 是独立域名，改成：new Uri("https://api.yoursite.com/")

//});
// ✅ 2. 【关键】手动注册 BaseAddressAuthorizationMessageHandler
builder.Services.AddScoped<BaseAddressAuthorizationMessageHandler>();

// ✅ 3. 配置命名 HttpClient
builder.Services.AddHttpClient("api-client", client =>
{
    client.BaseAddress = new Uri("http://localhost:5086/"); // 你的后端 API 地址
})
.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>(); // 自动添加 Bearer Token

// ✅ 4. 注册 Typed HttpClient，用于 @inject HttpClient
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("api-client"));

builder.Services.AddScoped<AuthService>();
// 新闻管理
builder.Services.AddScoped<NewsService>();

// ✅ 5. 正确注册 PersistentAuthenticationStateProvider（使用工厂）
builder.Services.AddScoped<PersistentAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<PersistentAuthenticationStateProvider>());

// ✅ 6. 【关键】注册 IAccessTokenProvider 实现
builder.Services.AddScoped<IAccessTokenProvider>(sp =>
{
    var authStateProvider = sp.GetRequiredService<PersistentAuthenticationStateProvider>();
    var jsRuntime = sp.GetRequiredService<IJSRuntime>();
    return new AccessTokenProviderImpl(authStateProvider, jsRuntime);
});



await builder.Build().RunAsync();
