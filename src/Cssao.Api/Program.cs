
using Cssao.api.Configuration;
using Cssao.api.Middleware;
using Cssao.Api.Services;
using Cssao.Application.Features.News.Queries;
using Cssao.Domain.Services;
using Cssao.Domain.IRepositories;
using Cssao.Infrastructure.Data;
using Cssao.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Cssao.Infrastructure.AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Cssao.Api.Cssao.api
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
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Cssao API",
                    Version = "v1",
                    Description = "后台管理 API"
                });

                // ✅ 添加 JWT 认证支持
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "请输入 JWT Token，格式: Bearer {token}",
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            // Program.cs (.NET 6+)
            builder.Services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(GetNewsDetailQuery).Assembly));

            // 🔹 2. 添加 AutoMapper
            builder.Services.AddAutoMapper(
                typeof(MappingProfile) // 🔹 传入 Profile 类型，自动扫描该程序集下的所有 Profile
            );

            //builder.Services.AddScoped<NewsService>();  // 注册应用服务
            // 注册 IHttpContextAccessor（需要）
            builder.Services.AddHttpContextAccessor();

            // 注册当前用户服务
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            // Cssao.Web/Program.cs
            builder.Services.AddScoped<INewsRepository, NewsRepository>();

            // 注册 DbContext，使用 builder.Configuration 获取连接字符串
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")),
                    b => b.MigrationsAssembly("Cssao.Infrastructure") // ✅ 关键：指定迁移项目
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

            // ✅ 添加 JWT 认证
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                    };

                    // ✅ 允许查询字符串传 token（调试用）
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;

                            // ✅ 只有在 /hub 路径时才从 query 取 token
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub"))
                            {
                                context.Token = accessToken;
                            }

                            // ✅ 其他情况，让默认行为从 Header 读取 Authorization
                            return Task.CompletedTask;
                        },

                        // 🔥 添加日志，方便调试
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine("JWT 认证失败: " + context.Exception.Message);
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            Console.WriteLine("✅ Token 验证成功");
                            var user = context.Principal;
                            Console.WriteLine("👤 用户名: " + user.Identity.Name);
                            Console.WriteLine("🔑 Claims: " + string.Join(", ", user.Claims.Select(c => c.Type + "=" + c.Value)));
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            Console.WriteLine("💡 Challenge 触发: " + context.Error + " " + context.ErrorDescription);
                            return Task.CompletedTask;
                        }
                    };
                });

            var app = builder.Build();
            // 🔹 HTTPS 重定向
            //app.UseHttpsRedirection();
            app.UseRouting();

            // 启用 CORS 中间件（必须在 UseAuthorization 之前）
            app.UseCors("AllowFrontend");
          
            // 🔹 认证 & 授权（如果有）
            app.UseAuthentication();
          
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
            // 🔴 在 UseAuthentication 之后，MapControllers 之前添加
            app.UseMiddleware<BlacklistedTokenMiddleware>();
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
