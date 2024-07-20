using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Dal.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Dal.Repositories;

public class UserRepository(BoomTokenContext boomTokenContext) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken ct)
    {
        if (await boomTokenContext.User.AsNoTracking().AnyAsync(u => u.TelegramId == user.TelegramId, ct))
            throw new ArgumentException("Данный пользователь зарегистрирован");
        await boomTokenContext.AddAsync(user, ct);
    }

    public async Task DeleteByIdAsync(Guid id, CancellationToken ct)
    {
        var userFound = await GetAsync(u => u.Id == id, ct);
        if (userFound == null)
            throw new ArgumentNullException("Данного пользователя не было найденно");
        boomTokenContext.User.Remove(userFound);
    }

    public async Task<IEnumerable<User>> GetAllAsync(
        Expression<Func<User, bool>>? predicate,
        CancellationToken ct,
        params Expression<Func<User, object>>[] includes)
    {
        var users = boomTokenContext.User.AsNoTracking();
        if (includes.Length != 0)
            Array.ForEach(includes, i => users = users.Include(i));
        if (predicate != null)
            users = users.Where(predicate);

        return await users.ToListAsync(ct);
    }

    public async Task UpdateAsync(User user, CancellationToken ct)
    {
        var userFound = await GetAsync(u => u.Id == user.Id, ct);
        if (userFound == null) 
            throw new ArgumentNullException(nameof(userFound));
        boomTokenContext.User.Update(user);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
        => await boomTokenContext.SaveChangesAsync(ct);

    public async Task<User?> GetAsync(
        Expression<Func<User, bool>> predicate,
        CancellationToken ct,
        params Expression<Func<User, object>>[] includes)
    {
        var users = boomTokenContext.User.AsNoTracking();
        if (includes.Length != 0)
            Array.ForEach(includes, i => users = users.Include(i));

        return await users.FirstOrDefaultAsync(predicate, ct);
    }
}
