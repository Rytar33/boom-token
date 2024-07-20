using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Dal.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Dal.Repositories;

public class TaskForRewardRepository(BoomTokenContext context) : ITaskForRewardRepository
{
    public async Task AddAsync(TaskForReward entity, CancellationToken ct)
        => await context.TaskForReward.AddAsync(entity, ct);
    

    public async Task DeleteByIdAsync(Guid id, CancellationToken ct)
    {
        var taskForReward = await GetAsync(t => t.Id == id, ct);
        if (taskForReward == null)
            throw new ArgumentNullException("Данного задания не было найденно");
        context.TaskForReward.Remove(taskForReward);
    }

    public async Task<IEnumerable<TaskForReward>> GetAllAsync(
        Expression<Func<TaskForReward, bool>>? predicate,
        CancellationToken ct,
        params Expression<Func<TaskForReward, object>>[] includes)
    {
        var tasksForReward = context.TaskForReward.AsNoTracking();
        if (includes != null)
            Array.ForEach(includes, i => tasksForReward = tasksForReward.Include(i));
        if (predicate != null)
            tasksForReward = tasksForReward.Where(predicate);
        return await tasksForReward.ToListAsync(ct);
    }

    public async Task<TaskForReward?> GetAsync(
        Expression<Func<TaskForReward, bool>> predicate,
        CancellationToken ct,
        params Expression<Func<TaskForReward, object>>[] includes)
    {
        var tasksForReward = context.TaskForReward.AsNoTracking();
        if (includes != null)
            Array.ForEach(includes, i => tasksForReward = tasksForReward.Include(i));
        return await tasksForReward.FirstOrDefaultAsync(predicate, ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
        => await context.SaveChangesAsync(ct);

    public async Task UpdateAsync(TaskForReward entity, CancellationToken ct)
    {
        var taskForReward = await GetAsync(t => t.Id == entity.Id, ct);
        if (taskForReward == null)
            throw new ArgumentNullException("Данного задания не было найденно");
        context.TaskForReward.Update(entity);
    }
}
