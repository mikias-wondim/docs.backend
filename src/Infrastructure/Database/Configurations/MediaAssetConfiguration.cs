using Domain.Media;
using Infrastructure.Database.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public sealed class MediaAssetConfiguration : EntityConfiguration<MediaAsset>
{
    protected override void ConfigureEntity(EntityTypeBuilder<MediaAsset> builder)
    {
        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(MediaAssetConstraints.MaxNameLength);

        builder.Property(m => m.AltName)
            .HasMaxLength(MediaAssetConstraints.MaxAltNameLength);        
        
        builder.Property(m => m.Url)
            .IsRequired()
            .HasMaxLength(MediaAssetConstraints.MaxUrlLength);

        builder.Property(m => m.Type)
            .IsRequired()
            .HasMaxLength(MediaAssetConstraints.MaxTypeLength);

        builder.HasOne(m => m.Section)
            .WithMany(s => s.MediaAssets)
            .HasForeignKey(m => m.SectionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
