using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Configurations;

// Configures the FavoriteAlbum entity, including table mapping, composite key, and relationships.
public class FavoriteAlbumConfiguration : IEntityTypeConfiguration<FavoriteAlbum>
{
    public void Configure(EntityTypeBuilder<FavoriteAlbum> builder)
    {
        builder.ToTable("favorite_albums");  
        builder.HasKey(f => new { f.ProfileId, f.AlbumId }); // composite primary key for FavoriteAlbum

        builder.HasOne(f => f.Profile).WithMany(p => p.FavoriteAlbums) // many to one relationship
            .HasForeignKey(f => f.ProfileId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(f => f.Album).WithMany() // many to one relationship
            .HasForeignKey(f => f.AlbumId).OnDelete(DeleteBehavior.Cascade);
    }
}