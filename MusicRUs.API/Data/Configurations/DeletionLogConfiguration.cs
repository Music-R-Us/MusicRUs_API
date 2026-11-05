using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Configurations;

public class DeletionLogConfiguration : IEntityTypeConfiguration<DeletionLog>
{
    // Configures the DeletionLog entity, including table mapping and property settings.
    public void Configure(EntityTypeBuilder<DeletionLog> builder)
    {
        builder.ToTable("deletion_logs");

        builder.Property(d => d.TableName).HasMaxLength(100);
        builder.Property(d => d.EntityId).HasMaxLength(100);

        builder.HasIndex(d => new { d.TableName, d.EntityId });
        builder.HasIndex(d => d.DeletedAt);
    }
}