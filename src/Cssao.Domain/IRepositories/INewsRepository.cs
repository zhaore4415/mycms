using Cssao.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Cssao.Domain.IRepositories
{
    public interface INewsRepository
    {
        Task<IPagedList<News>> GetByCategoryAsync(int categoryId, int pageIndex, int pageSize, CancellationToken ct);
        Task<News> GetByIdWithCommentsAsync(int id, CancellationToken ct);
        Task AddAsync(News news, CancellationToken ct);
        Task UpdateAsync(News news, CancellationToken ct);
    }

}
