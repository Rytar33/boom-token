using Domain.Entities;
using Domain.Primitives.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Dal.EntityFramework.Configurations;

public class TaskForRewardConfiguration : IEntityTypeConfiguration<TaskForReward>
{
    public void Configure(EntityTypeBuilder<TaskForReward> builder)
    {
        builder.ToTable("task_for_reward");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("name");

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(250)
            .HasColumnName("description");

        builder.Property(t => t.TargetType)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("target_type");

        builder.Property(t => t.TaskType)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("task_type");

        builder.Property(t => t.Link)
            .HasMaxLength(1000)
            .HasColumnName("link");

        builder.Property(t => t.ProgressToCompletion)
            .IsRequired()
            .HasDefaultValue(0)
            .HasColumnName("progress_to_completion");

        builder.Property(t => t.Reward)
            .IsRequired()
            .HasColumnName("reward");

        builder.Property(t => t.IsActive)
            .HasDefaultValue(true)
            .IsRequired()
            .HasColumnName("is_active");

        builder.HasData([
            new TaskForReward("Канал LOREM", "", TaskType.Single, TargetType.OpenLink, 1, 1000, "https://t.me/rutar33") { Id = Guid.NewGuid() },
            new TaskForReward("Канал ISPUM", "", TaskType.Single, TargetType.OpenLink, 1, 1000, "https://t.me/Dartist1") { Id = Guid.NewGuid() },
            new TaskForReward("Канал OLEG", "", TaskType.Single, TargetType.OpenLink, 1, 1000, "https://t.me/Dartist1") { Id = Guid.NewGuid() },
            new TaskForReward("Накликать 1.5к раз", "", TaskType.EveryDay, TargetType.Tap, 1500, 3000) { Id = Guid.NewGuid() },
            new TaskForReward("Пригласить друга", "", TaskType.EveryDay, TargetType.ReferalInvite, 1, 3000) { Id = Guid.NewGuid() },
            new TaskForReward("Канал Boom token", "", TaskType.EveryDay, TargetType.OpenLink, 1, 500, "https://t.me/rutar33") { Id = Guid.NewGuid() },
        ]);
    }
}
