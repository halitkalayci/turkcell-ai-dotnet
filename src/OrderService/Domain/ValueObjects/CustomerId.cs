using OrderService.Domain.Common;

namespace OrderService.Domain.ValueObjects;

/// <summary>
/// Value object representing a customer identifier.
/// Provides type safety and prevents mixing customer IDs with other GUIDs.
/// </summary>
public sealed class CustomerId : ValueObject
{
    public Guid Value { get; }

    private CustomerId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Customer ID cannot be empty", nameof(value));

        Value = value;
    }

    public static CustomerId Create(Guid value)
    {
        return new CustomerId(value);
    }

    public static CustomerId CreateNew()
    {
        return new CustomerId(Guid.NewGuid());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    // Implicit conversion for convenience
    public static implicit operator Guid(CustomerId customerId)
    {
        return customerId.Value;
    }
}
