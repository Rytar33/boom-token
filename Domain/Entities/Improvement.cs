using Domain.Primitives.Enums;

namespace Domain.Entities;

public class Improvement : BaseEntity
{
    public Improvement(
        string name,
        string description,
        ImprovementType improvementType,
        int valueIncreased,
        short level,
        int cost)
    { 
        Name = name;
        Description = description;
        ImprovementType = improvementType;
        ValueIncreased = valueIncreased;
        Level = level;
        Cost = cost;
    }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public ImprovementType ImprovementType { get; private set; }

    public int ValueIncreased { get; private set; }

    public short Level { get; private set; }

    public int Cost { get; private set; }

    public void Update(
        string? name = null,
        string? description = null,
        ImprovementType? improvementType = null,
        int? valueIncreased = null,
        short? level = null,
        int? cost = null)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;
        if (!string.IsNullOrWhiteSpace(description))
            Description = description;
        if (improvementType != null)
            ImprovementType = improvementType.Value;
        if (valueIncreased != null)
            ValueIncreased = valueIncreased.Value;
        if (level != null)
            Level = level.Value;
        if (cost != null)
            Cost = cost.Value;
    }
}
