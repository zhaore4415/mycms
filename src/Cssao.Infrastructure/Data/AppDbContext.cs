using Cssao.Domain.Services;
using Cssao.Domain.Entities;
using Cssao.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Cssao.Infrastructure.Data
{
    /// <summary>
    /// Infrastructure/Data/NewsDbContext.cs
    /// 应用程序数据库上下文，负责数据访问和审计逻辑。
    /// </summary>
    public class AppDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService currentUserService)
            : base(options)
        {
            _currentUserService = currentUserService;
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
        //dotnet ef database update --project ../Cssao.Infrastructure/Cssao.Infrastructure.csproj --startup-project ./Cssao.Api.csproj
        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // 全局过滤：自动排除已软删除的新闻
            modelBuilder.Entity<News>()
                .HasQueryFilter(n => !n.IsDeleted);

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

            // 其他配置...
            base.OnModelCreating(modelBuilder);

        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var currentUser = _currentUserService.IsAuthenticated
            ? _currentUserService.UserName
            : "system";

            // 处理新增和修改的审计字段
            foreach (var entry in ChangeTracker.Entries<IAuditable>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        if (entry.Entity is ICreatedBy createdBy)
                            createdBy.CreatedBy = currentUser;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        if (entry.Entity is IUpdatedBy updatedBy)
                            updatedBy.UpdatedBy = currentUser;
                        break;
                }
            }

            // 处理软删除（标记为已删除）
            foreach (var entry in ChangeTracker.Entries<ISoftDelete>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified; // 拦截删除操作
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;

                    // 如果实体支持更新人，则记录
                    if (entry.Entity is IUpdatedBy updatedBy)
                        updatedBy.UpdatedBy = currentUser;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
