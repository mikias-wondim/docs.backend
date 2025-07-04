using Domain.Faqs;
using Infrastructure.Database.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public sealed class FaqConfiguration : EntityConfiguration<Faq>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Faq> builder)
    {
                builder.Property(f => f.Question)
                    .IsRequired()
                    .HasMaxLength(FaqConstraints.MaxQuestionLength);
        
                builder.Property(f => f.Answer)
                    .IsRequired()
                    .HasMaxLength(FaqConstraints.MaxAnswerLength);
        
                builder.Property(p => p.Order)
                    .IsRequired()
                    .HasPrecision(16, 6);
        
                builder.HasOne(f => f.Page)
                    .WithMany(p => p.Faqs)
                    .HasForeignKey(f => f.PageId)
                    .OnDelete(DeleteBehavior.Cascade);
    }
}
