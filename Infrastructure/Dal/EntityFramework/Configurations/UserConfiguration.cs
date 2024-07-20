using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Dal.EntityFramework.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(p => p.TelegramId)
            .IsRequired()
            .HasColumnName("telegram_id");

        builder.Property(p => p.Balance)
            .IsRequired()
            .HasDefaultValue(0)
            .HasColumnName("balance");

        builder.Property(p => p.RangTap)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasColumnName("rang_tap");

        builder.Property(p => p.LimitEnergy)
            .IsRequired()
            .HasDefaultValue(500)
            .HasColumnName("limit_energy");

        builder.Property(p => p.Energy)
            .IsRequired()
            .HasDefaultValue(500)
            .HasColumnName("energy");

        builder.Property(p => p.EnergyRecoveryInSecond)
            .IsRequired()
            .HasDefaultValue(1)
            .HasColumnName("energy_recovery_in_second");

        builder.Property(p => p.RewardForClick)
            .IsRequired()
            .HasDefaultValue(1)
            .HasColumnName("reward_for_click");

        builder.Property(p => p.CountClick)
            .IsRequired()
            .HasDefaultValue(0)
            .HasColumnName("count_click");

        builder.Property(p => p.ReferalLink)
            .IsRequired()
            .HasMaxLength(20)
            .HasColumnName("referal_link");

        builder.Property(p => p.DateTimeRegistration)
            .IsRequired()
            .HasColumnType("timestamp without time zone")
            .HasColumnName("date_time_registration");
    }
}
