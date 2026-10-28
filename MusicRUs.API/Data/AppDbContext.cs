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
    public DbSet<Artist> Artists => Set<Artist>(); // Artists in the system
    public DbSet<Album> Albums => Set<Album>(); // Albums of artists
    public DbSet<Track> Tracks => Set<Track>(); // Tracks of albums

    // Profiles (step 4)
    public DbSet<ForumProfile> ForumProfiles => Set<ForumProfile>(); // Forum profiles of users
    public DbSet<FavoriteArtist> FavoriteArtists => Set<FavoriteArtist>(); // Favorite artists of users
    public DbSet<FavoriteAlbum> FavoriteAlbums => Set<FavoriteAlbum>(); // Favorite albums of users
    
    // Music Collections (step 5)
    public DbSet<MusicCollection> MusicCollections => Set<MusicCollection>(); // Music collections of users

    // Genres (step 6)
    public DbSet<Genre> Genres => Set<Genre>(); // Music genres in the system

    // Comments (step 7)
    public DbSet<Comment> Comments => Set<Comment>(); // Comments on forum posts
    public DbSet<ForumPost> ForumPosts => Set<ForumPost>(); // Forum posts in the system

    // Artist Subscriptions (step 8)
    public DbSet<Concert> Concerts => Set<Concert>(); // Concerts in the system
    public DbSet<ArtistSubscription> ArtistSubscriptions => Set<ArtistSubscription>(); // Artist subscriptions of users


    
    protected override void OnModelCreating(ModelBuilder modelBuilder) 
    {
        modelBuilder.HasPostgresExtension("citext"); // Enable citext extension for case-insensitive text columns
        modelBuilder.HasPostgresExtension("pg_trgm"); // Trigram index for fast ILIKE '%text%' search (FK-003)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly); 
    }
}