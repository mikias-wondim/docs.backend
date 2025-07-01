using Domain.Sections;
using Infrastructure.Database.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel;

namespace Infrastructure.Database.Configurations;

internal sealed class SectionUserAccessConfiguration : EntityConfiguration<SectionUserAccess>
{
    protected override void ConfigureEntity(EntityTypeBuilder<SectionUserAccess> builder)
    {
        builder.HasKey(su => su.Id);

        builder.Property(su => su.SectionId)
            .IsRequired();

        builder.Property(su => su.UserId)
            .IsRequired();

        builder.HasIndex(su => new { su.SectionId, su.UserId })
            .IsUnique();

        builder.HasOne(su => su.User)
            .WithMany()
            .HasForeignKey(su => su.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(su => su.User.RecordStatus != RecordStatus.Deleted);
    }
}
