namespace Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; }

    public override bool Equals(object? obj)
        => obj is BaseEntity entity 
        && entity.Id == Id
        && entity.GetHashCode() == GetHashCode();

    public override int GetHashCode()
        => Id.GetHashCode();
}