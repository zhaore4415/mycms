using Cssao.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Cssao.Infrastructure.Data
{
    // Infrastructure/Data/NewsDbContext.cs
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<BlacklistedToken> BlacklistedTokens { get; set; }


        public DbSet<News> News { get; set; }  // 实体映射
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }

        #region Code First Migration（代码优先迁移）
        //# 1. 进入启动项目目录
        //        cd D:\南禾网站建设\mywebcode\cssaoNew\cssapapi\src\Cssao.Api

        //# 2. 生成迁移（Migration）
        //dotnet ef migrations add CreateBlacklistedTokenTable --project ../Cssao.Infrastructure/Cssao.Infrastructure.csproj --startup-project ./Cssao.Api.csproj --output-dir Migrations

        //# 3. 更新数据库
        //dotnet ef database update --project ../Cssao.Infrastructure/Cssao.Infrastructure.csproj --startup-project./Cssao.Api.csproj
        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //// 配置实体关系或约束
            //modelBuilder.Entity<News>().HasKey(n => n.Id);

            // 配置实体关系和约束
            modelBuilder.Entity<News>(entity =>
            {
                entity.HasKey(e => e.Id);

                // 配置与 Category 的关系
                entity.HasOne(d => d.Category)
                    .WithMany(p => p.News)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_News_Category");
            });
        }
    }

}
