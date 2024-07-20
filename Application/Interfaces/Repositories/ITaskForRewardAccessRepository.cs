using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ITaskForRewardAccessRepository : IRepository<TaskForRewardAccess>
{
    Task RemoveRange(IEnumerable<TaskForRewardAccess> entities);

    Task AddRangeAsync(IEnumerable<TaskForRewardAccess> entities, CancellationToken ct);
}
