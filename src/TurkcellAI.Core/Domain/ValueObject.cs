using System.Collections.Generic;
using System.Linq;

namespace TurkcellAI.Core.Domain;

public abstract class ValueObject
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(0, (hash, component) =>
            {
                var componentHash = component?.GetHashCode() ?? 0;
                unchecked
                {
                    return (hash * 397) ^ componentHash;
                }
            });
    }
}
