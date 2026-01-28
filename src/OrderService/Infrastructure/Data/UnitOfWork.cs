using OrderService.Application.Ports;

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

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
