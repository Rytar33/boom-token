using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Dal.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Dal.Repositories;

public class ImprovementRepository(BoomTokenContext context) : IImprovementRepository
{
    public async Task AddAsync(
        Improvement entity,
        CancellationToken ct)
    {
        if (await context.Improvement.AnyAsync(
                e => e.ImprovementType == entity.ImprovementType
                     && e.Level == entity.Level
                     && e.Id != entity.Id, ct))
            throw new ArgumentException("Данный уровень у этого улучшения присутствует");

        await context.AddAsync(entity, ct);
    }

    public async Task DeleteByIdAsync(Guid id, CancellationToken ct)
    {
        var improvement = await GetAsync(i => i.Id == id, ct);
        if (improvement == null)
            throw new ArgumentNullException("Данного улучшение не было найдено");
        context.Improvement.Remove(improvement);
    }

    public async Task<IEnumerable<Improvement>> GetAllAsync(
        Expression<Func<Improvement, bool>>? predicate,
        CancellationToken ct,
        params Expression<Func<Improvement, object>>[] includes)
    {
        var improvements = context.Improvement.AsNoTracking();
        if (includes.Length != 0)
            Array.ForEach(includes, i => improvements = improvements.Include(i));
        if (predicate != null)
            improvements = improvements.Where(predicate);
        return await improvements.ToListAsync(ct);
    }

    public async Task<Improvement?> GetAsync(
        Expression<Func<Improvement, bool>> predicate,
        CancellationToken ct,
        params Expression<Func<Improvement, object>>[] includes)
    {
        var improvements = context.Improvement.AsNoTracking();
        if (includes.Length != 0)
            Array.ForEach(includes, i => improvements = improvements.Include(i));
        return await improvements.FirstOrDefaultAsync(predicate, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
        => context.SaveChangesAsync(ct);

    public async Task UpdateAsync(Improvement entity, CancellationToken ct)
    {
        if (!await context.Improvement.AnyAsync(e => e.Id == entity.Id, ct))
            throw new ArgumentNullException("Данного улучшение не было найдено");
        if (await context.Improvement.AnyAsync(
            e => e.ImprovementType == entity.ImprovementType
                && e.Level == entity.Level
                && e.Id != entity.Id, ct))
            throw new ArgumentException("Данный уровень у этого улучшения присутствует");

        context.Improvement.Update(entity);
    }
}
