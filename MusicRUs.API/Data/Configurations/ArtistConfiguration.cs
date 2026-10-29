using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Configurations;

public class ArtistConfiguration : IEntityTypeConfiguration<Artist>
{
    // Configuration for the Artist entity, including table mapping, property constraints, and relationships.
    public void Configure(EntityTypeBuilder<Artist> builder)
    {
        builder.ToTable("artists");
        builder.Property(a => a.SpotifyId).HasMaxLength(22); // Spotify's 22-character ID
        builder.HasIndex(a => a.SpotifyId).IsUnique(); // Ensure SpotifyId is unique
        builder.Property(a => a.Name).HasMaxLength(255); 
        builder.HasIndex(a => a.Name).HasMethod("gin").HasOperators("gin_trgm_ops"); // Trigram index for fast ILIKE '%text%' search on artist name

        builder.HasMany(a => a.Genres) // many to many relationship
       .WithMany(g => g.Artists)
       .UsingEntity<ArtistGenre>(
           r => r.HasOne<Genre>().WithMany().HasForeignKey(x => x.GenreId)
                 .OnDelete(DeleteBehavior.Restrict),
           l => l.HasOne<Artist>().WithMany().HasForeignKey(x => x.ArtistId)
                 .OnDelete(DeleteBehavior.Cascade),
           j =>
           {
               j.ToTable("artist_genres");
               j.HasKey(x => new { x.ArtistId, x.GenreId });
           });
           
           builder.Property(a => a.TicketmasterId).HasMaxLength(50); // Ticketmaster's ID for the artist
           builder.HasIndex(a => a.TicketmasterId).IsUnique(); // Ensure TicketmasterId is unique
    }
}
