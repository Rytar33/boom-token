using Domain.Entities;
using Infrastructure.Dal.EntityFramework.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Dal.EntityFramework;

public class BoomTokenContext : DbContext
{
    public BoomTokenContext(DbContextOptions<BoomTokenContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Improvement> Improvement { get; set; }

    public DbSet<ImprovementAccess> ImprovementAccess { get; set; }

    public DbSet<ReferalUsers> ReferalUsers { get; set; }

    public DbSet<TaskForReward> TaskForReward { get; set; }

    public DbSet<TaskForRewardAccess> TaskForRewardAccess { get; set; }

    public DbSet<User> User { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ImprovementAccessConfiguration());
        modelBuilder.ApplyConfiguration(new ImprovementConfiguration());
        modelBuilder.ApplyConfiguration(new ReferalUsersConfiguration());
        modelBuilder.ApplyConfiguration(new TaskForRewardAccessConfiguration());
        modelBuilder.ApplyConfiguration(new TaskForRewardConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}
