using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Dal.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Dal.Repositories;

public class TaskForRewardAccessRepository(BoomTokenContext context) : ITaskForRewardAccessRepository
{
    public async Task AddAsync(TaskForRewardAccess entity, CancellationToken ct)
    {
        if (await context.TaskForRewardAccess.AnyAsync(
            t => t.IdTaskForReward == entity.IdTaskForReward
                && t.IdUser == entity.IdUser, ct))
            throw new ArgumentException("Это задание уже присутствует у пользователя");
        if (entity.DateTimeCompleted != null || entity.CurrentValue != 0)
            throw new ArgumentException("Невозможно сразу выполнить принятое задание");

        await context.TaskForRewardAccess.AddAsync(entity, ct);
    }

    public async Task DeleteByIdAsync(Guid id, CancellationToken ct)
    {
        var taskForRewardAccess = await GetAsync(t => t.Id == id, ct);
        if (taskForRewardAccess == null)
            throw new ArgumentNullException("Данного задание не было найденно");
        context.TaskForRewardAccess.Remove(taskForRewardAccess);
    }

    public async Task<IEnumerable<TaskForRewardAccess>> GetAllAsync(
        Expression<Func<TaskForRewardAccess, bool>>? predicate,
        CancellationToken ct,
        params Expression<Func<TaskForRewardAccess, object>>[] includes)
    {
        var tasksForRewardAccess = context.TaskForRewardAccess.AsQueryable();
        if (includes.Length != 0)
            Array.ForEach(includes, i => tasksForRewardAccess = tasksForRewardAccess.Include(i));
        if (predicate != null)
            tasksForRewardAccess = tasksForRewardAccess.Where(predicate);
        return await tasksForRewardAccess.ToListAsync(ct);
    }

    public async Task<TaskForRewardAccess?> GetAsync(
        Expression<Func<TaskForRewardAccess, bool>> predicate,
        CancellationToken ct,
        params Expression<Func<TaskForRewardAccess, object>>[] includes)
    {
        var tasksForRewardAccess = context.TaskForRewardAccess.AsNoTracking();
        if (includes.Length != 0)
            Array.ForEach(includes, i => tasksForRewardAccess = tasksForRewardAccess.Include(i));
        var taskForRewardAccess = await tasksForRewardAccess.FirstOrDefaultAsync(predicate, ct);
        return taskForRewardAccess;
    }

    public async Task SaveChangesAsync(CancellationToken ct)
        => await context.SaveChangesAsync(ct);

    public async Task UpdateAsync(TaskForRewardAccess entity, CancellationToken ct)
    {
        var taskForRewardAccess = await GetAsync(t => t.Id == entity.Id, ct);
        if (taskForRewardAccess == null)
            throw new ArgumentNullException("Данного задания не было найденно");
        context.TaskForRewardAccess.Update(entity);
    }

    public Task RemoveRange(IEnumerable<TaskForRewardAccess> entities)
    {
        context.TaskForRewardAccess.RemoveRange(entities);
        return Task.CompletedTask;
    }

    public async Task AddRangeAsync(IEnumerable<TaskForRewardAccess> entities, CancellationToken ct)
        => await context.TaskForRewardAccess.AddRangeAsync(entities, ct);
    
}
