using Domain.Entities;
using Domain.Primitives.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Dal.EntityFramework.Configurations;

public class ImprovementConfiguration : IEntityTypeConfiguration<Improvement>
{
    public void Configure(EntityTypeBuilder<Improvement> builder)
    {
        builder.ToTable("improvement");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("name");

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(250)
            .HasColumnName("description");

        builder.Property(p => p.ImprovementType)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("improvement_type");

        builder.Property(p => p.ValueIncreased)
            .IsRequired()
            .HasColumnName("value_increased");

        builder.Property(p => p.Cost)
            .IsRequired()
            .HasColumnName("cost");

        builder.Property(p => p.Level)
            .IsRequired()
            .HasColumnName("level");

        builder.HasData([
            new Improvement("Energy limit lvl 1", "", ImprovementType.EnergyLimit, 50, 1, 5000) { Id = Guid.NewGuid() },
            new Improvement("Energy limit lvl 2", "", ImprovementType.EnergyLimit, 50, 2, 10000) { Id = Guid.NewGuid() },
            new Improvement("Energy limit lvl 3", "", ImprovementType.EnergyLimit, 100, 3, 20000) { Id = Guid.NewGuid() },
            new Improvement("Energy limit lvl 4", "", ImprovementType.EnergyLimit, 150, 4, 35000) { Id = Guid.NewGuid() },
            new Improvement("Energy limit lvl 5", "", ImprovementType.EnergyLimit, 250, 5, 50000) { Id = Guid.NewGuid() },
            new Improvement("Energy recovery lvl 1", "", ImprovementType.SpeedEnergyRecovery, 1, 1, 10000) { Id = Guid.NewGuid() },
            new Improvement("Energy recovery lvl 2", "", ImprovementType.SpeedEnergyRecovery, 1, 2, 20000) { Id = Guid.NewGuid() },
            new Improvement("Energy recovery lvl 3", "", ImprovementType.SpeedEnergyRecovery, 2, 3, 50000) { Id = Guid.NewGuid() },
            new Improvement("Energy recovery lvl 4", "", ImprovementType.SpeedEnergyRecovery, 2, 4, 100000) { Id = Guid.NewGuid() },
            new Improvement("Energy recovery lvl 5", "", ImprovementType.SpeedEnergyRecovery, 5, 5, 200000) { Id = Guid.NewGuid() },
            new Improvement("Tap profit lvl 1", "", ImprovementType.ProfitPerTap, 1, 1, 10000) { Id = Guid.NewGuid() },
            new Improvement("Tap profit lvl 2", "", ImprovementType.ProfitPerTap, 1, 2, 20000) { Id = Guid.NewGuid() },
            new Improvement("Tap profit lvl 3", "", ImprovementType.ProfitPerTap, 1, 3, 50000) { Id = Guid.NewGuid() },
            new Improvement("Tap profit lvl 4", "", ImprovementType.ProfitPerTap, 2, 4, 150000) { Id = Guid.NewGuid() },
            new Improvement("Tap profit lvl 5", "", ImprovementType.ProfitPerTap, 2, 5, 250000) { Id = Guid.NewGuid() } ]);
    }
}
