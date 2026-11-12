using MusicRUs.API.DTOs;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Services.Interfaces;

public interface IMusicCollectionService
{
    Task<MusicCollectionResponseDto?> AddToOwnedCollectionAsync(int userId,CreateOwnedMusicCollectionDto dto); // CREATE (Owned collection)
    Task<MusicCollectionResponseDto?> AddToWantedCollectionAsync(int userId,CreateWantedMusicCollectionDto dto); // CREATE (Wanted collection)
    Task<IEnumerable<MusicCollectionResponseDto>> GetOwnedCollectionByUserIdAsync(int userId); // READ Owned
    Task<IEnumerable<MusicCollectionResponseDto>> GetWantedCollectionByUserIdAsync(int userId); // READ Wanted
    Task<MusicCollectionResponseDto?> GetCollectionItemAsync(int userId, int collectionId); // READ
    Task<MusicCollectionResponseDto?> UpdateCollectionItemAsync(int userId, int collectionId, UpdateMusicCollectionDto dto); // UPDATE
    Task<bool> DeleteCollectionItemAsync(int userId, int collectionId); // DELETE
}