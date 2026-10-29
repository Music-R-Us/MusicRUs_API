using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Configurations;

// Configuration for the Comment entity, including table mapping, constraints, and relationships.
public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder) 
    {
        builder.ToTable("comments", t => t.HasCheckConstraint(
            "ck_comments_not_empty", "char_length(btrim(content)) > 0"));

        builder.HasIndex(c => new { c.PostId, c.CreatedAt }); // Index to optimize queries fetching  

        builder.HasOne(c => c.Post).WithMany(p => p.Comments).HasForeignKey(c => c.PostId) // One to many relationship
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(c => c.User).WithMany().HasForeignKey(c => c.UserId)  // One to many relationship
               .OnDelete(DeleteBehavior.Cascade);
    }
}