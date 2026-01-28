namespace OrderService.Domain.Common;

/// <summary>
/// Base class for aggregate roots in the domain model.
/// Aggregate roots are the entry points for modifying a group of related entities.
/// </summary>
/// <typeparam name="TId">The type of the aggregate root's identifier</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    protected AggregateRoot(TId id) : base(id)
    {
    }

    // Required for EF Core
    protected AggregateRoot() : base()
    {
    }
}
