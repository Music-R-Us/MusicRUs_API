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

        builder.Property(p => p.Title).HasMaxLength(255).IsRequired();
        builder.HasIndex(p => p.CreatedAt); 

        builder.HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId) // many to one relationship
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Track).WithMany().HasForeignKey(p => p.TrackId)
               .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(p => p.Album).WithMany().HasForeignKey(p => p.AlbumId)
               .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(p => p.Artist).WithMany().HasForeignKey(p => p.ArtistId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(p => p.Genres) // many to many relationship
               .WithMany(g => g.Posts) // many to many relationship
               .UsingEntity<PostGenre>(
                   r => r.HasOne<Genre>().WithMany().HasForeignKey(x => x.GenreId)
                         .OnDelete(DeleteBehavior.Restrict),
                   l => l.HasOne<ForumPost>().WithMany().HasForeignKey(x => x.PostId)
                         .OnDelete(DeleteBehavior.Cascade),
                   j =>
                   {
                       j.ToTable("post_genres"); // join table many to many relationship
                       j.HasKey(x => new { x.PostId, x.GenreId });
                   });
    }
}