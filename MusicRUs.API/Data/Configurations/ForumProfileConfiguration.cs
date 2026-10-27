using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Configurations;

public class ForumProfileConfiguration : IEntityTypeConfiguration<ForumProfile>
{
    // Configures the ForumProfile entity, including table mapping, property constraints, and relationships.
    public void Configure(EntityTypeBuilder<ForumProfile> builder)
    {
        builder.ToTable("forum_profiles");
        builder.Property(p => p.DisplayName).HasMaxLength(100); 
        builder.Property(p => p.Location).HasMaxLength(255); 

        // One-to-one: EF puts a unique index on user_id, so a user can have at most one profile
        builder.HasOne(p => p.User) // Configure the one-to-one relationship with User
            .WithOne(u => u.Profile) // Configure the inverse navigation property on User
            .HasForeignKey<ForumProfile>(p => p.UserId) // Set the foreign key to the UserId property of ForumProfile
            .OnDelete(DeleteBehavior.Cascade); // Delete the profile when the user is deleted
    }
}