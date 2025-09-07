using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cssao.Domain.Services;

namespace Cssao.Infrastructure.Data
{
    public class NewsDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            //// 获取 Cssao.api 项目目录（即包含 appsettings.json 的目录）
            //var apiProjectPath = Path.Combine(
            //    Directory.GetCurrentDirectory(),
            //    "..",  "Cssao.Api"); // 根据实际目录结构调整

            //// 设置配置文件路径（根据实际项目结构调整）
            //var configuration = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json")
            //    .Build();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                options => options.MigrationsAssembly("Cssao.Infrastructure")
            );
            // ✅ 创建一个设计时用的“假” CurrentUserService 实现
            var currentUserService = new DesignTimeCurrentUserService();
            return new AppDbContext(optionsBuilder.Options, currentUserService);
        }
        // ✅ 设计时专用的实现（仅用于迁移）
        private class DesignTimeCurrentUserService : ICurrentUserService
        {
            public string? UserId => "design-time-user";
            public string? UserName => "migrations";
            public bool IsAuthenticated => true;
        }

    }
}
