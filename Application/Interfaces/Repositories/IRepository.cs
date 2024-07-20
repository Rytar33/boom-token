using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Interfaces.Repositories;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? predicate,
        CancellationToken ct,
        params Expression<Func<TEntity, object>>[] includes);

    Task<TEntity?> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct,
        params Expression<Func<TEntity, object>>[] includes);

    Task AddAsync(TEntity entity, CancellationToken ct);

    Task UpdateAsync(TEntity entity, CancellationToken ct);

    Task DeleteByIdAsync(Guid id, CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);
}
