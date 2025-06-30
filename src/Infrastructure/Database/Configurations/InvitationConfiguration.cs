using Domain.Invitations;
using Infrastructure.Database.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel;

namespace Infrastructure.Database.Configurations;

internal sealed class InvitationConfiguration: EntityConfiguration<Invitation>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Invitation> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.ProjectId)
            .IsRequired();

        builder.Property(i => i.InvitedUserId)
            .IsRequired();

        builder.Property(i => i.InvitedByUserId)
            .IsRequired();
        
        builder.Property(i => i.Role)
            .IsRequired();
        
        builder.Property(i => i.SentAt)
            .IsRequired();
        
        builder.Property(i => i.ExpiresAt)
            .IsRequired();
        
        builder.Property(i => i.Status)
            .IsRequired();
        
        builder.HasIndex(i => new { i.ProjectId, i.InvitedUserId, i.InvitedByUserId })
            .IsUnique();
        
        builder.HasIndex(i => new { i.ProjectId, i.InvitedUserId, i.Status })
            .IsUnique();

        // Relationships
        builder.HasOne(i => i.Project)
            .WithMany(p => p.Invitations)
            .HasForeignKey(i => i.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(i => i.InvitedUser)
            .WithMany(u => u.Invitations)
            .HasForeignKey(i => i.InvitedUserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasQueryFilter(r => r.InvitedUser.RecordStatus != RecordStatus.Deleted);
    }
}
