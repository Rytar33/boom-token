using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Dal.EntityFramework.Configurations;

public class ImprovementAccessConfiguration : IEntityTypeConfiguration<ImprovementAccess>
{
    public void Configure(EntityTypeBuilder<ImprovementAccess> builder)
    {
        builder.ToTable("improvement_access");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(p => p.IdUser)
            .IsRequired()
            .HasColumnName("id_user");

        builder.Property(p => p.IdImprovement)
            .IsRequired()
            .HasColumnName("id_improvement");

        builder.HasOne(u => u.User)
            .WithMany()
            .HasForeignKey(u => u.IdUser);

        builder.HasOne(i => i.Improvement)
            .WithMany()
            .HasForeignKey(i => i.IdImprovement);
    }
}
