using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Configurations;

public class ArtistSubscriptionConfiguration : IEntityTypeConfiguration<ArtistSubscription>
{
    // Configures the ArtistSubscription entity and its relationships with User and Artist entities.
    public void Configure(EntityTypeBuilder<ArtistSubscription> builder)
    {
        builder.ToTable("artist_subscriptions"); // Maps the entity to the "artist_subscriptions" table
        builder.HasKey(s => new { s.UserId, s.ArtistId }); // Composite primary key on UserId and ArtistId

        builder.Property(s => s.CountryCode).HasMaxLength(2).IsFixedLength(); // Country code, empty means "all" (UC-012)
        builder.Property(s => s.Region).HasMaxLength(100); // Region or state of the concert location
        builder.Property(s => s.City).HasMaxLength(100); // City of the concert location

        builder.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId)
               .OnDelete(DeleteBehavior.Cascade); // Configures the foreign key relationship with the User entity and sets cascade delete behavior
        builder.HasOne(s => s.Artist).WithMany().HasForeignKey(s => s.ArtistId)
               .OnDelete(DeleteBehavior.Cascade); // Configures the foreign key relationship with the Artist entity and sets cascade delete behavior
    }
}