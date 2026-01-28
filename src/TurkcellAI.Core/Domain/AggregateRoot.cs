namespace TurkcellAI.Core.Domain;

public abstract class AggregateRoot<TId> : Entity<TId>
{
    protected AggregateRoot(TId id) : base(id) { }

    // Parameterless constructor for EF Core
    protected AggregateRoot() : base() { }
}
