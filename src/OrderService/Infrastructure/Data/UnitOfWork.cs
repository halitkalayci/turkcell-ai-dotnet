using Microsoft.EntityFrameworkCore.Storage;
using TurkcellAI.Core.Application.Abstractions;

namespace OrderService.Infrastructure.Data;

/// <summary>
/// Unit of Work implementation wrapping the DbContext.
/// Manages database transactions and coordinates persistence operations.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly OrderDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(OrderDbContext context)
    {
        _context = context;
    }

    // Transaction methods for Core IUnitOfWork (SQL Server)
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}
