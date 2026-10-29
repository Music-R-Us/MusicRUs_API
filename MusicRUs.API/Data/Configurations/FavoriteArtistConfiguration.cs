using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Configurations;

// Configures the FavoriteArtist entity, including table mapping, composite key, and relationships.
public class FavoriteArtistConfiguration : IEntityTypeConfiguration<FavoriteArtist>
{
    public void Configure(EntityTypeBuilder<FavoriteArtist> builder)
    {
        builder.ToTable("favorite_artists");  
        builder.HasKey(f => new { f.ProfileId, f.ArtistId }); // Set the composite primary key for FavoriteArtist

        builder.HasOne(f => f.Profile).WithMany(p => p.FavoriteArtists) // many to one relationship
            .HasForeignKey(f => f.ProfileId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(f => f.Artist).WithMany() // many to one relationship
            .HasForeignKey(f => f.ArtistId).OnDelete(DeleteBehavior.Cascade);
    }
}