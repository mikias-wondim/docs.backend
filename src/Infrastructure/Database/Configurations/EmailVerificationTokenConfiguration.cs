using Domain.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel;

namespace Infrastructure.Database.Configurations;

internal sealed class EmailVerificationTokenConfiguration: IEntityTypeConfiguration<EmailVerificationToken>
{
    public void Configure(EntityTypeBuilder<EmailVerificationToken> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Token)
            .HasMaxLength(255)
            .IsRequired();
        
        builder.HasIndex(e => e.Token)
            .IsUnique();
        
        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .IsRequired();
        
        builder.HasQueryFilter(r => r.User.RecordStatus != RecordStatus.Deleted);
    }
}
