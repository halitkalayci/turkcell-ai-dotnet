namespace OrderService.Application.Ports;

/// <summary>
/// Unit of Work interface for managing database transactions.
/// Provides transactional boundaries for aggregate persistence.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves all changes made in the current transaction.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
