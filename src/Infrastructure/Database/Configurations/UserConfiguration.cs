using Domain.Users;
using Infrastructure.Database.Configurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class UserConfiguration : EntityConfiguration<User>
{
    protected override void ConfigureEntity(EntityTypeBuilder<User> builder)
    {
        // === Core Identity Fields ===
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(UserConstraints.MaxEmailLength);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(UserConstraints.MaxFirstNameLength);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(UserConstraints.MaxLastNameLength);
        
        builder.Property(u => u.DisplayName)
            .HasMaxLength(UserConstraints.MaxDisplayNameLength);

        builder.Property(u => u.AvatarUrl)
            .HasMaxLength(UserConstraints.MaxAvatarUrlLength);

        builder.Property(u => u.Bio)
            .HasMaxLength(UserConstraints.MaxBioLength);

        // === Boolean + Date ===
        builder.Property(u => u.EmailVerified)
            .IsRequired();

        builder.Property(u => u.LastLoginAt);
    }
}
