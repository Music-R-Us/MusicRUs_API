using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Interceptors;


// EF Core interceptor that automatically sets CreatedAt and UpdatedAt
// timestamps on entities implementing "IHasTimestamps" just before changes are saved to the database.
// Register it on the DbContext like: options.AddInterceptors(new TimestampInterceptor()).
// This keeps timestamp logic in one place instead of in every repository or service.
public sealed class TimestampInterceptor : SaveChangesInterceptor
{
    /// Called by EF Core before a synchronous <c>SaveChanges()</c> call.
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        Stamp(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// Called by EF Core before an asynchronous SaveChangesAsync() call.
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        Stamp(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    // Sets timestamps on all tracked "IHasTimestamps" entities
    // "context" = the DbContext being saved - may be null.
    private static void Stamp(DbContext? context)
    {
        if (context is null) return;

        var now = DateTime.UtcNow;
        foreach (var entry in context.ChangeTracker.Entries<IHasTimestamps>())
        {

            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = now;

            if (entry.State is EntityState.Added or EntityState.Modified)
                entry.Entity.UpdatedAt = now;
        }
    }
}
