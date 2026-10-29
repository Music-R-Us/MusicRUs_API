using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Configurations;
// Configuration for the Album entity, including table mapping, property constraints, and relationships.
public class AlbumConfiguration : IEntityTypeConfiguration<Album>
{
    public void Configure(EntityTypeBuilder<Album> builder) 
    {
        builder.ToTable("albums"); 
        builder.Property(a => a.SpotifyId).HasMaxLength(22); // Spotify's 22-character ID
        builder.HasIndex(a => a.SpotifyId).IsUnique(); // Ensure SpotifyId is unique
        builder.Property(a => a.Title).HasMaxLength(255); 
        builder.Property(a => a.AlbumType).HasConversion<string>().HasMaxLength(20); 
        builder.Property(a => a.ReleaseDate).HasMaxLength(10);

        builder.HasMany(a => a.Artists) // many to many relationship
            .WithMany(ar => ar.Albums)
            .UsingEntity<AlbumArtist>(
                r => r.HasOne<Artist>().WithMany().HasForeignKey(x => x.ArtistId),
                l => l.HasOne<Album>().WithMany().HasForeignKey(x => x.AlbumId),
                j =>
                {
                    j.HasKey(x => new { x.AlbumId, x.ArtistId });
                    j.ToTable("album_artists");
                });

        builder.HasMany(a => a.Tracks) // one to many relationship
            .WithOne(t => t.Album)
            .HasForeignKey(t => t.AlbumId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(a => a.Title).HasMethod("gin").HasOperators("gin_trgm_ops"); // Trigram index for fast ILIKE '%text%' search on album title
    }
}