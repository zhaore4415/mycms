using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cssao.Infrastructure.Data
{
    public class NewsDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // 获取 Cssao.api 项目目录（即包含 appsettings.json 的目录）
            var apiProjectPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",  "Cssao.api"); // 根据实际目录结构调整

            // 设置配置文件路径（根据实际项目结构调整）
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                options => options.MigrationsAssembly("Cssao.Infrastructure")
            );

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
