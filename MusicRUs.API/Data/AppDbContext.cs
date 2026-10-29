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
    
    // Music Collections (step 5)
    public DbSet<MusicCollection> MusicCollections => Set<MusicCollection>();

    // Genres (step 6)
    public DbSet<Genre> Genres => Set<Genre>();
    // Comments (step 7)
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<ForumPost> ForumPosts => Set<ForumPost>();

    // Artist Subscriptions (step 8)
    public DbSet<Concert> Concerts => Set<Concert>();
    public DbSet<ArtistSubscription> ArtistSubscriptions => Set<ArtistSubscription>();

    // Friendships, User Blocks, and Messages (step 11)
    public DbSet<Friendship> Friendships => Set<Friendship>();
    public DbSet<UserBlock> UserBlocks => Set<UserBlock>();
    public DbSet<Message> Messages => Set<Message>();


    
    protected override void OnModelCreating(ModelBuilder modelBuilder) 
    {
        modelBuilder.HasPostgresExtension("citext"); // Enable citext extension for case-insensitive text columns
        modelBuilder.HasPostgresExtension("pg_trgm"); // Trigram index for fast ILIKE '%text%' search (FK-003)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly); 
    }
}