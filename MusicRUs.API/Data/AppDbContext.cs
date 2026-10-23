using Microsoft.EntityFrameworkCore;

namespace MusicRUs.API.Data;

public class AppDbContext : DbContext
{
    // Constructor to initialize DbContext with options
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {} 


    
    protected override void OnModelCreating(ModelBuilder modelBuilder) 
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly); 
    }
}