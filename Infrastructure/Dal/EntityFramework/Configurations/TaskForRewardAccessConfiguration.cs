using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Dal.EntityFramework.Configurations;

public class TaskForRewardAccessConfiguration : IEntityTypeConfiguration<TaskForRewardAccess>
{
    public void Configure(EntityTypeBuilder<TaskForRewardAccess> builder)
    {
        builder.ToTable("task_for_reward_access");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(t => t.IdUser)
            .IsRequired()
            .HasColumnName("id_user");

        builder.Property(t => t.IdTaskForReward)
            .IsRequired()
            .HasColumnName("id_task_for_reward");

        builder.Property(t => t.CurrentValue)
            .IsRequired()
            .HasDefaultValue(0)
            .HasColumnName("current_value");

        builder.Property(t => t.DateTimeCompleted)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("date_time_completed");

        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.IdUser);

        builder.HasOne(t => t.TaskForReward)
            .WithMany()
            .HasForeignKey(t => t.IdTaskForReward);
    }
}
