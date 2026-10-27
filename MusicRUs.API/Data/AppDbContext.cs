using Microsoft.EntityFrameworkCore;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data;

public class AppDbContext : DbContext
{
    // Constructor to initialize DbContext with options
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {} 

    // Users (step 1)
    public DbSet<User> Users => Set<User>();
    
    // Artists and Albums (step 2)
    public DbSet<Artist> Artists => Set<Artist>();
    public DbSet<Album> Albums => Set<Album>();
    public DbSet<Track> Tracks => Set<Track>();
    // Profiles (step 4)
    public DbSet<ForumProfile> ForumProfiles => Set<ForumProfile>();
    public DbSet<FavoriteArtist> FavoriteArtists => Set<FavoriteArtist>();
    public DbSet<FavoriteAlbum> FavoriteAlbums => Set<FavoriteAlbum>();

    
    protected override void OnModelCreating(ModelBuilder modelBuilder) 
    {
        modelBuilder.HasPostgresExtension("citext"); // Enable citext extension for case-insensitive text columns
        modelBuilder.HasPostgresExtension("pg_trgm"); // Trigram index for fast ILIKE '%text%' search (FK-003)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly); 
    }
}