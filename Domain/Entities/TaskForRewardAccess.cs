namespace Domain.Entities;

public class TaskForRewardAccess : BaseEntity
{
    public TaskForRewardAccess(Guid idUser, Guid idTaskForReward)
    {
        IdUser = idUser;
        IdTaskForReward = idTaskForReward;
        CurrentValue = 0;
    }
    
    public Guid IdUser { get; private set; }

    public Guid IdTaskForReward { get; private set; }

    public int CurrentValue { get; private set; }

    public DateTime? DateTimeCompleted { get; private set; }

    public User? User { get; private set; }

    public TaskForReward? TaskForReward { get; private set; }

    public void Update(
        int? currentValue = null,
        DateTime? dateTimeCompleted = null)
    {
        if (currentValue != null)
            CurrentValue = currentValue.Value;
        if (dateTimeCompleted != null)
            DateTimeCompleted = dateTimeCompleted.Value;
    }
}
