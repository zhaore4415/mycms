
using Cssao.api.Configuration;
using Cssao.Application.Services;
using Cssao.Domain.IRepositories;
using Cssao.Infrastructure.Data;
using Cssao.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace cssaoapi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //var configuration = new ConfigurationBuilder()
            //.SetBasePath(Directory.GetCurrentDirectory())
            //.AddJsonFile("appsettings.json")
            //.Build();
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<NewsService>();  // 注册应用服务
                                                        // Cssao.Web/Program.cs
            builder.Services.AddScoped<INewsRepository, NewsRepository>();

            // 注册 DbContext，使用 builder.Configuration 获取连接字符串
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
                )
            );

            // ✅ 注册 JWT 配置
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

            // ✅ 添加 CORS（前端在 localhost:5173 或 3000）
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:5200", "http://localhost:5173") // 改成你的前端地址
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            var app = builder.Build();
            // 🔹 HTTPS 重定向
            //app.UseHttpsRedirection();
            app.UseCors(); // ✅ 在 MapControllers 之前
            app.UseRouting();


            // 🔹 认证 & 授权（如果有）
            app.UseAuthentication();
            // 启用 CORS 中间件（必须在 UseAuthorization 之前）
            app.UseCors("AllowFrontend");
            app.UseAuthorization();
            // ✅ Swagger 放在 UseRouting 之后，MapControllers 之前
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            // ✅ 全局处理 OPTIONS 请求
            app.MapMethods("/api/admin/auth/login", new[] { "OPTIONS" }, () => Results.Ok());

            // 或者更通用的方式：为所有 API 路由支持 OPTIONS
            app.Use((context, next) =>
            {
                if (context.Request.Method == "OPTIONS")
                {
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "https://localhost:7251");
                    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                    context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
                    context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                    context.Response.StatusCode = 200;
                    return context.Response.WriteAsync("OK");
                }
                return next();
            });

            app.MapControllers();

            // 执行数据库迁移
            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    dbContext.Database.Migrate();
                    Console.WriteLine("数据库迁移成功");
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "执行数据库迁移时出错");
                    throw;
                }
            }

            app.Run();


        }
    }
}
