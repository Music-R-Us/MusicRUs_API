using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Configurations;

public class TrackConfiguration : IEntityTypeConfiguration<Track>
{
    // Configuration for the Track entity, including table mapping, property constraints, and relationships.
    public void Configure(EntityTypeBuilder<Track> builder)
    {
        builder.ToTable("tracks");
        builder.Property(t => t.SpotifyId).HasMaxLength(22); // Spotify's 22-character ID
        builder.HasIndex(t => t.SpotifyId).IsUnique(); // Ensure SpotifyId is unique

        builder.HasMany(t => t.Artists) // many to many relationship
            .WithMany(a => a.Tracks)
            .UsingEntity<TrackArtist>( 
                r => r.HasOne<Artist>().WithMany().HasForeignKey(x => x.ArtistId),
                l => l.HasOne<Track>().WithMany().HasForeignKey(x => x.TrackId),
                j =>
                {
                    j.HasKey(x => new { x.TrackId, x.ArtistId });
                    j.ToTable("track_artists");
                });
    }
}