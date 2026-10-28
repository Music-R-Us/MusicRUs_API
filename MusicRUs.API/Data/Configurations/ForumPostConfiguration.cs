using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Configurations;

// Configuration for the ForumPost entity, including table mapping, constraints, and relationships.
public class ForumPostConfiguration : IEntityTypeConfiguration<ForumPost>
{
    public void Configure(EntityTypeBuilder<ForumPost> builder) 
    {
        builder.ToTable("forum_posts", t => // Configure table-level constraints and checks
        {
            t.HasCheckConstraint("ck_forum_posts_not_empty", 
                "char_length(btrim(title)) > 0 AND char_length(btrim(content)) > 0");
            t.HasCheckConstraint("ck_forum_posts_one_music_link",
                "num_nonnulls(track_id, album_id, artist_id) <= 1");
        });

        builder.Property(p => p.Title).HasMaxLength(255).IsRequired(); // Title is required and has a max length of 255
        builder.HasIndex(p => p.CreatedAt); 

        builder.HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId) // Configure the relationship with the User entity
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Track).WithMany().HasForeignKey(p => p.TrackId) // Configure the relationship with the Track entity
               .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(p => p.Album).WithMany().HasForeignKey(p => p.AlbumId) // Configure the relationship with the Album entity
               .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(p => p.Artist).WithMany().HasForeignKey(p => p.ArtistId) // Configure the relationship with the Artist entity
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(p => p.Genres) // Configure the many-to-many relationship with the Genre entity
               .WithMany(g => g.Posts) // Configure the join entity for the many-to-many relationship
               .UsingEntity<PostGenre>(
                   r => r.HasOne<Genre>().WithMany().HasForeignKey(x => x.GenreId) // Configure the relationship with the Genre entity in the join table
                         .OnDelete(DeleteBehavior.Restrict),
                   l => l.HasOne<ForumPost>().WithMany().HasForeignKey(x => x.PostId) // Configure the relationship with the ForumPost entity in the join table
                         .OnDelete(DeleteBehavior.Cascade),
                   j =>
                   {
                       j.ToTable("post_genres"); // Configure the join table for the many-to-many relationship
                       j.HasKey(x => new { x.PostId, x.GenreId }); // Configure the composite primary key for the join table
                   });
    }
}