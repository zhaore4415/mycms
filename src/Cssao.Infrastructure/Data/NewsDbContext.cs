using Cssao.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cssao.Infrastructure.Data
{
    // Infrastructure/Data/NewsDbContext.cs
    public class NewsDbContext : DbContext
    {
        public NewsDbContext(DbContextOptions<NewsDbContext> options)
            : base(options)
        {
        }

        public DbSet<News> News { get; set; }  // 实体映射
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }

        #region Code First Migration（代码优先迁移）
        //# 切换到 Infrastructure 项目目录
        //        cd D:\南禾网站建设\mywebcode\cssaoNew\cssapapi\src\Cssao.Infrastructure

        //# 再运行迁移命令
        //        dotnet ef migrations add InitialCreate --startup-project../Cssao.Api/Cssao.Api.csproj
        //        dotnet ef database update --startup-project../Cssao.Api/Cssao.Api.csproj 
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
