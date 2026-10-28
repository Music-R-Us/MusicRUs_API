using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Configurations;

// Configuration for the Comment entity, including table mapping, constraints, and relationships.
public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder) // Configure the Comment entity
    {
        builder.ToTable("comments", t => t.HasCheckConstraint( // Configure table-level constraints and checks
            "ck_comments_not_empty", "char_length(btrim(content)) > 0"));

        builder.HasIndex(c => new { c.PostId, c.CreatedAt }); // Index to optimize queries fetching comments for a post in chronological order  

        builder.HasOne(c => c.Post).WithMany(p => p.Comments).HasForeignKey(c => c.PostId) // Configure the relationship with the ForumPost entity
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(c => c.User).WithMany().HasForeignKey(c => c.UserId) // Configure the relationship with the User entity
               .OnDelete(DeleteBehavior.Cascade);
    }
}