using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Dal.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Dal.Repositories;

public class ReferalUsersRepository(BoomTokenContext context) : IReferalUsersRepository
{
    public async Task AddAsync(ReferalUsers entity, CancellationToken ct)
    {
        if (await context.ReferalUsers.AnyAsync(
            r => r.IdUser == entity.IdUser
                && r.IdUserInvited == entity.IdUserInvited, ct))
            throw new ArgumentException("Невозможно сделать связь, т.к. связь уже существует");
        if (await context.ReferalUsers.AnyAsync(
            r => r.IdUser == entity.IdUserInvited
                && r.IdUserInvited == entity.IdUser, ct))
            throw new ArgumentException("Невозможно сделать связь, между пользователями, т.к. один из вошел по реферальной ссылке");

        await context.AddAsync(entity, ct);
    }

    public async Task DeleteByIdAsync(Guid id, CancellationToken ct)
    {
        var referalUsers = await GetAsync(r => r.Id == id, ct);
        if (referalUsers == null)
            throw new ArgumentNullException("Реферальной ссылки не было найдено");
        context.ReferalUsers.Remove(referalUsers);
    }

    public async Task<IEnumerable<ReferalUsers>> GetAllAsync(
        Expression<Func<ReferalUsers, bool>>? predicate,
        CancellationToken ct,
        params Expression<Func<ReferalUsers, object>>[] includes)
    {
        var referalUsers = context.ReferalUsers.AsNoTracking();
        if (includes.Length != 0)
            Array.ForEach(includes, i => referalUsers = referalUsers.Include(i));
        if (predicate != null)
            referalUsers = referalUsers.Where(predicate);
        return await referalUsers.ToListAsync(ct);
    }

    public async Task<ReferalUsers?> GetAsync(
        Expression<Func<ReferalUsers, bool>> predicate,
        CancellationToken ct,
        params Expression<Func<ReferalUsers, object>>[] includes)
    {
        var referalUsers = context.ReferalUsers.AsNoTracking();
        if (includes.Length != 0)
            Array.ForEach(includes, i => referalUsers = referalUsers.Include(i));
        return await referalUsers.FirstOrDefaultAsync(predicate, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
        => context.SaveChangesAsync(ct);

    public async Task UpdateAsync(ReferalUsers entity, CancellationToken ct)
    {
        if (!await context.ReferalUsers.AnyAsync(r => r.Id == entity.Id, ct))
            throw new ArgumentNullException("Реферальной ссылки не было найдено");
        if (await context.ReferalUsers.AnyAsync(
            r => r.IdUser == entity.IdUserInvited
                && r.IdUserInvited == entity.IdUser, ct))
            throw new ArgumentException("Невозможно сделать связь, между пользователями, т.к. один из вошел по реферальной ссылке");

        context.ReferalUsers.Update(entity);
    }
}
