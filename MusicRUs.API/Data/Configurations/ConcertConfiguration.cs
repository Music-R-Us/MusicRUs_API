using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Configurations;

public class ConcertConfiguration : IEntityTypeConfiguration<Concert>
{
    // Configures the Concert entity for the database schema.
    public void Configure(EntityTypeBuilder<Concert> builder)
    {
        builder.ToTable("concerts"); // Maps the Concert entity to the "concerts" table in the database

        builder.Property(c => c.ExternalId).HasMaxLength(50); // Sets the maximum length for the ExternalId column
        builder.Property(c => c.Name).HasMaxLength(255); // Sets the maximum length for the Name column
        builder.Property(c => c.VenueName).HasMaxLength(255); // Sets the maximum length for the VenueName column
        builder.Property(c => c.City).HasMaxLength(100); // Sets the maximum length for the City column
        builder.Property(c => c.Region).HasMaxLength(100); // Sets the maximum length for the Region column
        builder.Property(c => c.CountryCode).HasMaxLength(2).IsFixedLength(); // Sets the maximum length and fixed length for the CountryCode column
        builder.Property(c => c.VenueTimeZone).HasMaxLength(64); // Sets the maximum length for the VenueTimeZone column

        builder.HasIndex(c => new { c.ArtistId, c.StartsAt }); // Creates an index on ArtistId and StartsAt columns
        // Same event only once per artist. Empty external_id (test data) is allowed
        builder.HasIndex(c => new { c.ArtistId, c.ExternalId }).IsUnique(); // Creates a unique index on ArtistId and ExternalId columns

        builder.HasOne(c => c.Artist).WithMany(a => a.Concerts).HasForeignKey(c => c.ArtistId) 
               .OnDelete(DeleteBehavior.Cascade); // Configures the foreign key relationship with the Artist entity and sets cascade delete behavior
    }
}