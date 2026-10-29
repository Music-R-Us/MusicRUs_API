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

        builder.HasOne(p => p.User) // one to one relationship
            .WithOne(u => u.Profile) 
            .HasForeignKey<ForumProfile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Genres) // many to many relationship
       .WithMany(g => g.Profiles) // many to many relationship
       .UsingEntity<ProfileGenre>(
           r => r.HasOne<Genre>().WithMany().HasForeignKey(x => x.GenreId)
                 .OnDelete(DeleteBehavior.Restrict),
           l => l.HasOne<ForumProfile>().WithMany().HasForeignKey(x => x.ProfileId) 
                 .OnDelete(DeleteBehavior.Cascade),
           j =>
           {
               j.ToTable("profile_genres");
               j.HasKey(x => new { x.ProfileId, x.GenreId });
           });
    }
}