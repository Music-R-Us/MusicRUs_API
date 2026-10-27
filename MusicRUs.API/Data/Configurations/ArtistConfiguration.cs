using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Configurations;

public class ArtistConfiguration : IEntityTypeConfiguration<Artist>
{
    // Configuration for the Artist entity, including table mapping, property constraints, and relationships.
    public void Configure(EntityTypeBuilder<Artist> builder)
    {
        builder.ToTable("artists"); // Map the Artist entity to the "artists" table
        builder.Property(a => a.SpotifyId).HasMaxLength(22); // Spotify's 22-character ID
        builder.HasIndex(a => a.SpotifyId).IsUnique(); // Ensure SpotifyId is unique
        builder.Property(a => a.Name).HasMaxLength(255); // Artist name with max length 255
        builder.HasIndex(a => a.Name).HasMethod("gin").HasOperators("gin_trgm_ops"); // Trigram index for fast ILIKE '%text%' search on artist name
    }
}
