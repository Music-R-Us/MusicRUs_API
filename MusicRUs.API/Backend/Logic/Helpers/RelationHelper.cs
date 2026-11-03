using MusicRUs.API.Repositories.Interfaces;

namespace MusicRUs.API.Backend.Helpers;

public static class RelationHelper 
{
    public static async Task<List<T>> GetAllOrThrowAsync<T>(
        IRepository<T> repo, IEnumerable<int> ids, string entityName) where T : class
    {
        var distinctIds = ids.Distinct().ToList();
        var entities = await repo.GetByIdsAsync(distinctIds); // Fetch entities by IDs
        if (entities.Count != distinctIds.Count)
            throw new Exception($"{entityName} not found");
        return entities;
    }

    public static void SyncOrdered<TJoin>( 
        ICollection<TJoin> current, // Current collection to be updated
        IEnumerable<int> ids, // New collection from client
        Func<TJoin, int> getId, // Return the ID of current item
        Func<int, TJoin> create, // Create a new item from given ID
        Action<TJoin, int> setOrder) // Set the display order of the item
    {
        var orderedIds = ids.Distinct().ToList();

        foreach (var row in current.Where(r => !orderedIds.Contains(getId(r))).ToList())
            current.Remove(row);

        for (var i = 0; i < orderedIds.Count; i ++)
        {
            var row = current.FirstOrDefault(r => getId(r) == orderedIds[i]);
            if (row == null)
            {
                row = create(orderedIds[i]);
                current.Add(row);
            }
            setOrder(row, i);
        }
    }
}