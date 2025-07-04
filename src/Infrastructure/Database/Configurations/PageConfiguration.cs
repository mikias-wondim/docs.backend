using Domain.Pages;
using Infrastructure.Database.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel;

namespace Infrastructure.Database.Configurations;

public sealed class PageConfiguration : EntityConfiguration<Page>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Page> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(PagesConstraints.MaxTitleLength);

        builder.Property(p => p.ContentMd)
            .HasMaxLength(PagesConstraints.MaxContentMdLength);

        builder.Property(p => p.Order)
            .HasPrecision(16, 6);

        builder.Ignore(p => p.Children);
        builder.Ignore(p => p.Tags);

        builder.Property<List<string>>("_tags")
            .HasColumnName("Tags")
            .HasConversion(
                v => string.Join(';', v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList())
            .Metadata.SetValueComparer(
                new ValueComparer<List<string>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));

        builder.HasOne(p => p.Section)
            .WithMany(s => s.Pages)
            .HasForeignKey(p => p.SectionId)
            .IsRequired();

        builder.HasOne(p => p.ParentPage)
            .WithMany()
            .HasForeignKey(p => p.ParentPageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(r => r.Section.RecordStatus != RecordStatus.Deleted);
    }
}
