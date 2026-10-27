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
        builder.HasKey(f => new { f.ProfileId, f.AlbumId }); // Set the composite primary key for FavoriteAlbum

        builder.HasOne(f => f.Profile).WithMany(p => p.FavoriteAlbums) // Configure the many-to-one relationship with ForumProfile
            .HasForeignKey(f => f.ProfileId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(f => f.Album).WithMany() // Configure the many-to-one relationship with Album
            .HasForeignKey(f => f.AlbumId).OnDelete(DeleteBehavior.Cascade);
    }
}