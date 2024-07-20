using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Dal.EntityFramework.Configurations;

public class ReferalUsersConfiguration : IEntityTypeConfiguration<ReferalUsers>
{
    public void Configure(EntityTypeBuilder<ReferalUsers> builder)
    {
        builder.ToTable("referal_users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(r => r.IdUser)
            .IsRequired()
            .HasColumnName("id_user");

        builder.Property(r => r.IdUserInvited)
            .IsRequired()
            .HasColumnName("id_user_invited");

        builder.Property(r => r.CountTakeFromClick)
            .IsRequired()
            .HasDefaultValue(0)
            .HasColumnName("count_take_from_click");

        builder.HasOne(u => u.User)
            .WithMany()
            .HasForeignKey(u => u.IdUser);

        builder.HasOne(u => u.UserInvited)
            .WithMany()
            .HasForeignKey(u => u.IdUserInvited);
    }
}
