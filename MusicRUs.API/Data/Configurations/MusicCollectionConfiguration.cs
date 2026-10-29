using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Configurations;

public class MusicCollectionConfiguration : IEntityTypeConfiguration<MusicCollection>
{
    // Configuration for the MusicCollection entity, including table mapping, property conversions, indexes, and relationships
    public void Configure(EntityTypeBuilder<MusicCollection> builder)
    {
        builder.ToTable("music_collections", t => t.HasCheckConstraint( 
            "ck_music_collections_wanted_without_condition",
            "status = 'Owned' OR condition IS NULL")); 

        builder.Property(c => c.Format).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.Condition).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(10);

        builder.HasIndex(c => new { c.UserId, c.AlbumId, c.Format }).IsUnique();

        builder.HasOne(c => c.User).WithMany().HasForeignKey(c => c.UserId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(c => c.Album).WithMany().HasForeignKey(c => c.AlbumId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}