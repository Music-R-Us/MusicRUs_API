using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Configurations;

public class ConcertConfiguration : IEntityTypeConfiguration<Concert>
{
    // Configuration for the Concert entity, including table mapping, property constraints, and relationships.
    public void Configure(EntityTypeBuilder<Concert> builder)
    {
        builder.ToTable("concerts"); 

        builder.Property(c => c.ExternalId).HasMaxLength(50);
        builder.Property(c => c.Name).HasMaxLength(255);
        builder.Property(c => c.VenueName).HasMaxLength(255);
        builder.Property(c => c.City).HasMaxLength(100);
        builder.Property(c => c.Region).HasMaxLength(100);
        builder.Property(c => c.CountryCode).HasMaxLength(2).IsFixedLength();
        builder.Property(c => c.VenueTimeZone).HasMaxLength(64);

        builder.HasIndex(c => new { c.ArtistId, c.StartsAt });
        // Same event only once per artist. Empty external_id (test data) is allowed
        builder.HasIndex(c => new { c.ArtistId, c.ExternalId }).IsUnique();

        builder.HasOne(c => c.Artist).WithMany(a => a.Concerts).HasForeignKey(c => c.ArtistId) 
               .OnDelete(DeleteBehavior.Cascade); // cascade delete behavior
    }
}