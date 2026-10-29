using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Configurations;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    // Configures the Genre entity, including table mapping, property settings, and indexes.
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable("genres");
        builder.Property(g => g.Id).HasIdentityOptions(startValue: 1000);
        builder.Property(g => g.Name).HasColumnType("citext");
        builder.HasIndex(g => g.Name).IsUnique();
    }
}