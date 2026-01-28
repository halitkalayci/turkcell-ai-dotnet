namespace ProductService.Domain.Entities;

/// <summary>
/// Base entity with unique identifier
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; protected set; }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
    }
}
