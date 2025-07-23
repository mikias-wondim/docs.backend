using Domain.Feedbacks;
using Infrastructure.Database.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

internal sealed class FeedbackConfiguration: EntityConfiguration<Feedback>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Feedback> builder)
    {
        builder.Property(f => f.Comment)
            .HasMaxLength(FeedbackConstraints.MaxCommentLength);

        builder.Property(f => f.Rating)
            .IsRequired();

        builder.Property(f => f.IsRead)
            .IsRequired();

        builder.Property(f => f.PageId)
            .IsRequired();

        builder.HasOne(f => f.Page)
            .WithMany(p => p.Feedbacks)
            .HasForeignKey(f => f.PageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
