using Cssao.Domain.Entities;
using Cssao.Domain.IRepositories;
using Cssao.Infrastructure.Data;
using Cssao.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

namespace Cssao.Infrastructure.Repositories
{
    // EF Core实现
    public class NewsRepository : INewsRepository
    {
        private readonly AppDbContext _context;

        public NewsRepository(AppDbContext context) => _context = context;

        public async Task AddAsync(News news)
        {
            if (news == null)
                throw new ArgumentNullException(nameof(news));

            _context.News.Add(news);
            await _context.SaveChangesAsync();
        }
   
        /// <summary>
        /// 更新新闻
        /// </summary>
        public async Task UpdateAsync(News news)
        {
            if (news == null)
                throw new ArgumentNullException(nameof(news));

            var existing = await _context.News.FindAsync(news.Id);
            if (existing == null)
                throw new KeyNotFoundException($"新闻ID {news.Id} 不存在");

            // 使用 EF Core 的跟踪机制
            _context.Entry(existing).CurrentValues.SetValues(news);

            // 如果有导航属性需要特别处理（如 Category、Tags），可额外配置
            // 或直接使用 Attach + Modified
            // _context.News.Attach(news);
            // _context.Entry(news).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        // Replace the following method implementation:

        /// <summary>
        /// 根据分类ID分页查询新闻列表（仅已发布）
        /// </summary>
        public async Task<IPagedList<News>> GetByCategoryAsync(int categoryId, int pageIndex, int pageSize)
        {
            var query = _context.News
                .Include(n => n.Category)           // 包含分类信息
                .Where(n => n.CategoryId == categoryId)
                .OrderByDescending(n => n.PublishDate);

            // Ensure the ToPagedListAsync extension method is available by referencing the correct package
            return  await query.ToPagedListAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// 根据ID查询新闻（包含评论和标签）
        /// </summary>
        public async Task<News?> GetByIdWithCommentsAsync(int id)
        {
            return await _context.News
                //.Include(n => n.Comments)
                //    .ThenInclude(c => c.User)      // 假设有评论用户
                //    .Where(c => c.AuditStatus == AuditStatus.Approved)
                .Include(n => n.Category)
                .Include(n => n.ArticleTags)
                    .ThenInclude(at => at.Tag)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        /// <summary>
        /// 根据ID获取新闻（不带导航属性，轻量）
        /// </summary>
        public async Task<News?> GetByIdAsync(int id)
        {
            return await _context.News.FindAsync(id);
        }

        /// <summary>
        /// 软删除或硬删除新闻
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                _context.News.Remove(news);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 获取所有新闻分页列表
        /// </summary>
        public async Task<IPagedList<News>> GetAllAsync(int pageIndex, int pageSize)
        {
            var query = _context.News
                .Include(n => n.Category)
                .OrderByDescending(n => n.PublishDate);
            //return default;
            return await query.ToPagedListAsync(pageIndex, pageSize);
        }
    }
}
