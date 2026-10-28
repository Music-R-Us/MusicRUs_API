using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Repositories.Interfaces;

public interface IMusicCollectionRepository : IRepository<MusicCollection>
{
    // Get a user's collection items filtered by status
    Task<IEnumerable<MusicCollection>> GetCollectionByUserIdAsync(int userId, CollectionStatus status); 

    // Get a specific collection item for a user
    Task<MusicCollection?> GetCollectionItemAsync(int userId, int collectionId); 

    // Check if a specific entry exists in the user's collection
    Task<bool> EntryExistAsync(int userId, int albumId, MediaFormat format); 
}