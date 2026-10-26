using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Configurations;

public class TrackConfiguration : IEntityTypeConfiguration<Track>
{
    // Configuration for the Track entity, including table mapping, property constraints, and relationships.
    public void Configure(EntityTypeBuilder<Track> builder)
    {
        builder.ToTable("tracks"); // Map the Track entity to the "tracks" table
        builder.Property(t => t.SpotifyId).HasMaxLength(22); // Spotify's 22-character ID
        builder.HasIndex(t => t.SpotifyId).IsUnique(); // Ensure SpotifyId is unique

        builder.HasMany(t => t.Artists) // Configure the many-to-many relationship between Track and Artist
            .WithMany(a => a.Tracks) // Configure the inverse relationship from Artist to Track
            .UsingEntity<TrackArtist>( 
                r => r.HasOne<Artist>().WithMany().HasForeignKey(x => x.ArtistId), // Configure the relationship from the join table to Artist
                l => l.HasOne<Track>().WithMany().HasForeignKey(x => x.TrackId), // Configure the relationship from the join table to Track
                j =>
                {
                    j.HasKey(x => new { x.TrackId, x.ArtistId }); // Composite primary key for the join table
                    j.ToTable("track_artists"); // Map the join table to "track_artists"
                });
    }
}