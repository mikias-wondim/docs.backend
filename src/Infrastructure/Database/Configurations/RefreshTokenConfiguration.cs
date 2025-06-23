using Domain.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel;

namespace Infrastructure.Database.Configurations;

internal sealed class RefreshTokenConfiguration: IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Token)
            .HasMaxLength(255)
            .IsRequired();

        builder.HasIndex(r => r.Token)
            .IsUnique();
        
        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .IsRequired();
        
        builder.HasQueryFilter(r => r.User.RecordStatus != RecordStatus.Deleted);
    }
}
