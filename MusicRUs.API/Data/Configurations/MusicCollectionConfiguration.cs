using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Configurations;

public class MusicCollectionConfiguration : IEntityTypeConfiguration<MusicCollection>
{
    // Configuration for the MusicCollection entity, including table mapping, property conversions, indexes, and relationships
    public void Configure(EntityTypeBuilder<MusicCollection> builder)
    {
        // Map the MusicCollection entity to the "music_collections" table with a check constraint for wanted items without condition
        builder.ToTable("music_collections", t => t.HasCheckConstraint( 
            "ck_music_collections_wanted_without_condition",
            "status = 'Owned' OR condition IS NULL"));

        builder.Property(c => c.Format).HasConversion<string>().HasMaxLength(20); // Convert the Format enum to a string with a maximum length of 20 characters
        builder.Property(c => c.Condition).HasConversion<string>().HasMaxLength(20); // Convert the Condition enum to a string with a maximum length of 20 characters
        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(10); // Convert the Status enum to a string with a maximum length of 10 characters

        builder.HasIndex(c => new { c.UserId, c.AlbumId, c.Format }).IsUnique(); // Ensure that each user can have only one entry per album and format combination

        builder.HasOne(c => c.User).WithMany().HasForeignKey(c => c.UserId)
               .OnDelete(DeleteBehavior.Cascade); // Configure the relationship with the User entity and cascade delete
        builder.HasOne(c => c.Album).WithMany().HasForeignKey(c => c.AlbumId)
               .OnDelete(DeleteBehavior.Restrict); // Configure the relationship with the Album entity and restrict delete
    }
}