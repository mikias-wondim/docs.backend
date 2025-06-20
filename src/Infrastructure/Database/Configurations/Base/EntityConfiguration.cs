using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel;

namespace Infrastructure.Database.Configurations.Base;

public abstract class EntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> 
    where TEntity : Entity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Primary Key
        builder.HasKey(e => e.Id);

        // Audit Properties
        builder.Property(e => e.StartDate)
            .IsRequired();

        builder.Property(e => e.EndDate)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(200);

        builder.Property(e => e.DeletedAt)
            .IsRequired();

        builder.Property(e => e.DeletedBy)
            .HasMaxLength(200);

        builder.Property(e => e.RecordStatus)
            .IsRequired();

        // Soft-delete global filter
        builder.HasQueryFilter(e => e.RecordStatus != RecordStatus.Deleted);

        ConfigureEntity(builder);
    }

    protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
}
