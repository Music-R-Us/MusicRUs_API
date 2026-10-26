using Microsoft.EntityFrameworkCore;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data;

public class AppDbContext : DbContext
{
    // Constructor to initialize DbContext with options
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {} 

    public DbSet<User> Users => Set<User>();
    public DbSet<Artist> Artists => Set<Artist>();
    public DbSet<Album> Albums => Set<Album>();
    public DbSet<Track> Tracks => Set<Track>();

    
    protected override void OnModelCreating(ModelBuilder modelBuilder) 
    {
        modelBuilder.HasPostgresExtension("citext"); // Enable citext extension for case-insensitive text columns
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly); 
    }
}