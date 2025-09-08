// Cssao.Infrastructure/UnitOfWork.cs
using Cssao.Domain;
using Cssao.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cssao.Infrastructure;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}