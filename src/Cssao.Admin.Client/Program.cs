using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Cssao.Admin.Client;
using Cssao.Admin.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5086/")//  https://localhost:7145/  默认指向 API 同源
    // 如果 API 是独立域名，改成：new Uri("https://api.yoursite.com/")
});


await builder.Build().RunAsync();
