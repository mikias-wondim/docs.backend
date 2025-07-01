using Domain.ProjectMembers;
using Infrastructure.Database.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel;

namespace Infrastructure.Database.Configurations;

internal sealed class ProjectMemberConfiguration: EntityConfiguration<ProjectMember>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ProjectMember> builder)
    {
        builder.HasKey(pm => pm.Id);

        builder.Property(pm => pm.ProjectId)
            .IsRequired();

        builder.Property(pm => pm.UserId)
            .IsRequired();

        builder.Property(pm => pm.Role)
            .IsRequired();
        builder.HasIndex(pm => new { pm.ProjectId, pm.UserId })
            .IsUnique();

        // Relationships
        builder.HasOne(pm => pm.Project)
            .WithMany(p => p.Members)
            .HasForeignKey(pm => pm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(pm => pm.User)
            .WithMany(u => u.ProjectMembers)
            .HasForeignKey(pm => pm.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasQueryFilter(r => r.User.RecordStatus != RecordStatus.Deleted);
        builder.HasQueryFilter(r => r.Project.RecordStatus != RecordStatus.Deleted);
    }
}
