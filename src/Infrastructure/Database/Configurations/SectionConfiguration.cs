using Domain.ProjectMembers;
using Domain.Sections;
using Infrastructure.Database.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class SectionConfiguration : EntityConfiguration<Section>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Section> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(SectionConstraints.MaxNameLength);

        builder.Property(s => s.Description)
            .HasMaxLength(SectionConstraints.MaxDescriptionLength);

        builder.Property(s => s.Visibility)
            .IsRequired();

        builder.Property(s => s.Password)
            .HasMaxLength(SectionConstraints.MaxPasswordLength);

        builder.Property(s => s.AllowedRoles)
            .HasConversion(
                v => v == null ? null : string.Join(',', v.Select(r => r.ToString())),
                v => string.IsNullOrWhiteSpace(v)
                    ? new List<ProjectRole>()
                    : v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(Enum.Parse<ProjectRole>)
                        .ToList()
            )
            .Metadata.SetValueComparer(new ValueComparer<List<ProjectRole>>(
                (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            ));


        builder.Property(s => s.Order)
            .IsRequired();

        // Relationships
        builder.HasOne(s => s.Project)
            .WithMany(p => p.Sections)
            .HasForeignKey(s => s.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.AllowedUsers)
            .WithOne(su => su.Section)
            .HasForeignKey(su => su.SectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
