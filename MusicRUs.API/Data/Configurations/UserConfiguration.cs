using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Configurations;

/// Configuration for the User entity, including property types, indexes, and check constraints.
/// Ensures that either a password hash or a Google ID is present for each user.
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.Username).HasColumnType("citext");
        //builder.Property(u => u.Email).HasColumnType("citext");   Removed as emails are now hashed and encrypted.
        builder.Property(u => u.EmailHash).HasMaxLength(64);
        builder.Property(u => u.GoogleId).HasMaxLength(255);

        builder.HasIndex(u => u.Username).IsUnique();
        //builder.HasIndex(u => u.Email).IsUnique();    Removed Email gets hashed and encrypted differently each time now.
        builder.HasIndex(u => u.EmailHash).IsUnique();
        builder.HasIndex(u => u.GoogleId).IsUnique();

        builder.ToTable(t => t.HasCheckConstraint(
            "ck_users_login_method",
            "password_hash IS NOT NULL OR google_id IS NOT NULL"));
    }
}