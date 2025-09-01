using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Cssao.Admin.Client;
using Cssao.Admin.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ✅ 1. 必须添加：授权核心服务
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5086/")//  https://localhost:7145/  默认指向 API 同源
    // 如果 API 是独立域名，改成：new Uri("https://api.yoursite.com/")
});

builder.Services.AddScoped<AuthService>();

// ✅ 4. 注册自定义认证状态提供者（关键！）
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

await builder.Build().RunAsync();
