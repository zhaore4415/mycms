using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
namespace Cssao.Shared
{
    /// <summary>
    /// IQueryable 异步分页扩展
    /// </summary>
    public static class PagedListExtensions
    {
        public static async Task<IPagedList<T>> ToPagedListAsync<T>(
            this IQueryable<T> source,
            int pageNumber,
            int pageSize)
        {
            // Ensure the Microsoft.EntityFrameworkCore namespace is used for CountAsync and ToListAsync
            var count = await source.CountAsync();
            var items = await source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new StaticPagedList<T>(items, pageNumber, pageSize, count);
        }
    }
}
