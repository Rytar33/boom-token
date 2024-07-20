using Domain.Primitives.Enums;

namespace Application.Dtos.Improvements;

public record ImprovementListItem(
    Guid Id,
    ImprovementType ImprovementType,
    string Name,
    int Cost,
    bool HasBuying);
