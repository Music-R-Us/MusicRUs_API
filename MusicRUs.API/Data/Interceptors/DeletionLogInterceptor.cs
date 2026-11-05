using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Interceptors;


// EF Core interceptor that logs every deleted entity (table name and primary key) just before changes are saved to the database.
// This keeps deletion logging logic in one place instead of in every repository or service.
public sealed class DeletionLogInterceptor : SaveChangesInterceptor
{
    // Called by EF Core before a synchronous <c>SaveChanges()</c> call.
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        LogDeletions(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    // Called by EF Core before an asynchronous SaveChangesAsync() call.
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        LogDeletions(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    // Logs deletions of entities by adding entries to the DeletionLog table, excluding the DeletionLog entity itself.
    private static void LogDeletions(DbContext? context)
    {
        if (context is null) return;

        var deleted = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Deleted && e.Entity is not DeletionLog)
            .ToList();      // ToList ensures we have a snapshot of deleted entries before we start adding to the DeletionLog table.

        var now = DateTime.UtcNow;

        foreach (var entry in deleted)
        {
            var keyValues = entry.Metadata.FindPrimaryKey()!.Properties
                .Select(p => entry.Property(p.Name).CurrentValue);

            context.Set<DeletionLog>().Add(new DeletionLog
            {
                TableName = entry.Metadata.GetTableName() ?? entry.Metadata.ClrType.Name,
                EntityId = string.Join(",", keyValues),
                DeletedAt = now
            });
        }
    }

}
