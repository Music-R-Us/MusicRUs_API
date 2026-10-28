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

        builder.HasMany(a => a.Genres) // Configure the many-to-many relationship between Artist and Genre
       .WithMany(g => g.Artists) // Configure the join entity for the many-to-many relationship
       .UsingEntity<ArtistGenre>(
           r => r.HasOne<Genre>().WithMany().HasForeignKey(x => x.GenreId) // Configure the foreign key for Genre in the join entity
                 .OnDelete(DeleteBehavior.Restrict),
           l => l.HasOne<Artist>().WithMany().HasForeignKey(x => x.ArtistId) // Configure the foreign key for Artist in the join entity
                 .OnDelete(DeleteBehavior.Cascade),
           j =>
           {
               j.ToTable("artist_genres"); // Map the join entity to the "artist_genres" table
               j.HasKey(x => new { x.ArtistId, x.GenreId }); // Set the composite primary key for the join entity
           });
    }
}
