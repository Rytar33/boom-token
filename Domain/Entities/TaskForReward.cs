using Domain.Primitives.Enums;

namespace Domain.Entities;

public class TaskForReward : BaseEntity
{
    public TaskForReward(
        string name,
        string description,
        TaskType taskType,
        TargetType targetType,
        int progressToCompletion,
        int reward,
        string? link = null)
    {
        Name = name;
        Description = description;
        TaskType = taskType;
        TargetType = targetType;
        Link = link;
        ProgressToCompletion = progressToCompletion;
        Reward = reward;
    }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public TaskType TaskType { get; private set; }

    public TargetType TargetType { get; private set; }

    public string? Link { get; private set; }

    public int ProgressToCompletion { get; private set; }

    public int Reward { get; private set; }

    public bool IsActive { get; private set; }

    public void Update(
        string? name = null, 
        string? description = null,
        TaskType? taskType = null,
        TargetType? targetType = null,
        string? link = null,
        int? progressToCompletion = null,
        int? reward = null,
        bool? isActive = null)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;
        if (!string.IsNullOrWhiteSpace(description))
            Description = description;
        if (taskType != null)
            TaskType = taskType.Value;
        if (targetType != null)
            TargetType = targetType.Value;
        if (!string.IsNullOrWhiteSpace(link))
            Link = link;
        if (progressToCompletion != null)
            ProgressToCompletion = progressToCompletion.Value;
        if (reward != null)
            Reward = reward.Value;
        if (isActive != null)
            IsActive = isActive.Value;
    }
}
