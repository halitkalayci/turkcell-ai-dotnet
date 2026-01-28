using TurkcellAI.Core.Application.Abstractions;

namespace OrderService.Infrastructure.Data;

/// <summary>
/// Unit of Work implementation wrapping the DbContext.
/// Manages database transactions and coordinates persistence operations.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly OrderDbContext _context;

    public UnitOfWork(OrderDbContext context)
    {
        _context = context;
    }

    // Transaction methods for Core IUnitOfWork (InMemory DB)
    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        // InMemory provider does not support real transactions; no-op
        return Task.CompletedTask;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        // InMemory provider does not support rollback; no-op
        return Task.CompletedTask;
    }
}
