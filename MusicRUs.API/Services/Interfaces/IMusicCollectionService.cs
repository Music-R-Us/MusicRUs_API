using MusicRUs.API.DTOs;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Services.Interfaces;

public interface IMusicCollectionService
{
    Task<MusicCollectionResponseDto?> AddToCollectionAsync(CreateMusicCollectionDto dto); // CREATE 
    Task<IEnumerable<MusicCollectionResponseDto>> GetCollectionByUserIdAsync(int userId); // READ all for specific userId
    Task<MusicCollectionResponseDto?> GetCollectionItemAsync(int userId, int collectionId); // READ
    Task<MusicCollectionResponseDto?> UpdateCollectionItemAsync(int userId, int collectionId, UpdateMusicCollectionDto dto); // UPDATE
    Task<bool> DeleteCollectionItemAsync(int userId, int collectionId); // DELETE
}