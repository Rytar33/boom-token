using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Dal.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Dal.Repositories;

public class ImprovementAccessRepository(BoomTokenContext context) : IImprovementAccessRepository
{
    public async Task AddAsync(ImprovementAccess entity, CancellationToken ct)
    {
        if (await context.ImprovementAccess.AnyAsync(
            i => i.IdImprovement == entity.IdImprovement
                && i.IdUser == entity.IdUser, ct))
            throw new ArgumentException("Данное улучшение приобретенно, невозможно купить.");
        
        await context.ImprovementAccess.AddAsync(entity, ct);
    }

    public async Task DeleteByIdAsync(Guid id, CancellationToken ct)
    {
        var improvementAccesses = await GetAsync(i => i.Id == id, ct);
        if (improvementAccesses == null)
            throw new ArgumentNullException("Данного улучшение не было найдено");
        context.ImprovementAccess.Remove(improvementAccesses);
    }

    public async Task<IEnumerable<ImprovementAccess>> GetAllAsync(
        Expression<Func<ImprovementAccess, bool>>? predicate,
        CancellationToken ct,
        params Expression<Func<ImprovementAccess, object>>[] includes)
    {
        var improvementAccesses = context.ImprovementAccess.AsNoTracking();
        if (includes.Length != 0)
            Array.ForEach(includes, i => improvementAccesses = improvementAccesses.Include(i));
        if (predicate != null)
            improvementAccesses = improvementAccesses.Where(predicate);

        return await improvementAccesses.ToListAsync(ct);
    }

    public async Task<ImprovementAccess?> GetAsync(
        Expression<Func<ImprovementAccess, bool>> predicate,
        CancellationToken ct,
        params Expression<Func<ImprovementAccess, object>>[] includes)
    {
        var improvementAccesses = context.ImprovementAccess.AsNoTracking();

        if (includes.Length != 0)
            Array.ForEach(includes, i => improvementAccesses = improvementAccesses.Include(i));

        return await improvementAccesses.FirstOrDefaultAsync(predicate, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
        => context.SaveChangesAsync(ct);

    public async Task UpdateAsync(ImprovementAccess entity, CancellationToken ct)
    {
        if (!await context.ImprovementAccess.AnyAsync(i => i.Id == entity.Id, ct))
            throw new ArgumentNullException("Данного улучшение не было найдено");
        if (await context.ImprovementAccess.AnyAsync(
            i => i.IdImprovement == entity.IdImprovement
                && i.IdUser == entity.IdUser, ct))
            throw new ArgumentException("Данное улучшение приобретенно, невозможно купить.");

        context.ImprovementAccess.Update(entity);
    }
}
