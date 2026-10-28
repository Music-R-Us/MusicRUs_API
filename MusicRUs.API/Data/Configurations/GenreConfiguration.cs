using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Configurations;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    // Configures the Genre entity, including table mapping, property settings, and indexes.
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable("genres"); // Maps the Genre entity to the "genres" table in the database
        builder.Property(g => g.Id).HasIdentityOptions(startValue: 1000); // Sets the starting value for the identity column
        builder.Property(g => g.Name).HasColumnType("citext"); // Uses case-insensitive text for the Name column
        builder.HasIndex(g => g.Name).IsUnique(); // Ensures the Name column has a unique index
    }
}