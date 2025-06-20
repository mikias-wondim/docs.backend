using Domain.Projects;
using Domain.Users;
using Infrastructure.Database.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class ProjectConfiguration: EntityConfiguration<Project>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Project> builder)
    {
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(ProjectConstraints.MaxNameLength);

        builder.Property(p => p.Description)
            .HasMaxLength(ProjectConstraints.MaxDescriptionLength);

        builder.Property(p => p.OverviewMd)
            .HasMaxLength(ProjectConstraints.MaxOverviewMdLength);

        builder.Property(p => p.Visibility)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany(u => u.Projects)
            .HasForeignKey(p => p.OwnerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // Optional: unique constraint for name per owner
        builder.HasIndex(p => new { p.OwnerId, p.Name })
            .IsUnique();
    }
}
