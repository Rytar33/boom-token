namespace Application.Dtos.TaskForRewards;

public record TaskListItem(
    Guid Id,
    string Name,
    int Reward,
    bool IsCompleted);
