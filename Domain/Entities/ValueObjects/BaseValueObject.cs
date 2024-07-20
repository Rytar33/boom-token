using System.Text.Json;

namespace Domain.Entities.ValueObjects;

public abstract class BaseValueObject
{
    public override bool Equals(object? obj)
        => obj is not null
        && obj is BaseValueObject valueObject
        && string.Compare(Serialize(valueObject), Serialize(this)) == 0;

    public override int GetHashCode()
        => Serialize(this).GetHashCode();

    private static string Serialize(BaseValueObject valueObject)
        => JsonSerializer.Serialize(valueObject);
}
