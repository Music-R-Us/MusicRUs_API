using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Configurations;

public class ArtistSubscriptionConfiguration : IEntityTypeConfiguration<ArtistSubscription>
{
    // Configuration for the ArtistSubscription entity, including table mapping, property constraints, and relationships.
    public void Configure(EntityTypeBuilder<ArtistSubscription> builder)
    {
        builder.ToTable("artist_subscriptions"); 
        builder.HasKey(s => new { s.UserId, s.ArtistId }); // Composite primary key on UserId and ArtistId

        builder.Property(s => s.CountryCode).HasMaxLength(2).IsFixedLength(); // Country code, empty means "all" (UC-012)
        builder.Property(s => s.Region).HasMaxLength(100);
        builder.Property(s => s.City).HasMaxLength(100);

        builder.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId)
               .OnDelete(DeleteBehavior.Cascade); // cascade delete behavior
        builder.HasOne(s => s.Artist).WithMany().HasForeignKey(s => s.ArtistId)
               .OnDelete(DeleteBehavior.Cascade); // cascade delete behavior
    }
}