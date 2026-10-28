using MusicRUs.API.DTOs;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;
using MusicRUs.API.Services.Interfaces;

namespace MusicRUs.API.Services;

public class MusicCollectionService : IMusicCollectionService
{
    private readonly IMusicCollectionRepository _mcRepo;

    public MusicCollectionService(IMusicCollectionRepository mcRepo)
    {
        _mcRepo = mcRepo;
    }

    // Implement the methods defined in IMusicCollectionService here

    public async Task<MusicCollectionResponseDto?> AddToCollectionAsync(CreateMusicCollectionDto dto) // CREATE
    {
        if (await _mcRepo.EntryExistAsync(dto.UserId, dto.AlbumId, dto.Format))
            return null!; // Entry already exists in the collection

        var item = new MusicCollection
        {
            UserId = dto.UserId,
            AlbumId = dto.AlbumId,
            Format = dto.Format,
            Condition = dto.Condition,
            Status = dto.Status
        };

        await _mcRepo.AddAsync(item);
        await _mcRepo.SaveChangesAsync();
        return MapToDto(item);
    }

    public async Task<IEnumerable<MusicCollectionResponseDto>> GetOwnedCollectionAsync(int userId) // READ Owned
    {
        var collection = await _mcRepo.GetCollectionByUserIdAsync(userId, CollectionStatus.Owned);
        return collection.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<MusicCollectionResponseDto>> GetWantedCollectionAsync(int userId) // READ Wanted
    {
        var collection = await _mcRepo.GetCollectionByUserIdAsync(userId, CollectionStatus.Wanted);
        return collection.Select(MapToDto).ToList();
    }

    public async Task<MusicCollectionResponseDto?> GetCollectionItemAsync(int userId, int collectionId) // READ
    {
        var item = await _mcRepo.GetCollectionItemAsync(userId, collectionId);
        return item == null ? null! : MapToDto(item);
    }

    public async Task<MusicCollectionResponseDto?> UpdateCollectionItemAsync(int userId, int collectionId, UpdateMusicCollectionDto dto) // UPDATE
    {
        var item = await _mcRepo.GetCollectionItemAsync(userId, collectionId);
        if(item == null)
            return null!;

        item.Format = dto.Format ?? item.Format; // Update fields if provided, else keep existing
        item.Condition = dto.Condition ?? item.Condition;
        item.Status = dto.Status ?? item.Status;
        item.UpdatedAt = DateTime.UtcNow;

        _mcRepo.Update(item);
        await _mcRepo.SaveChangesAsync();
        return MapToDto(item);
    }

    public async Task<bool> DeleteCollectionItemAsync(int userId, int collectionId) // DELETE
    {
        var item = await _mcRepo.GetCollectionItemAsync(userId, collectionId);
        if(item == null)
            return false;

        _mcRepo.Delete(item);
        await _mcRepo.SaveChangesAsync();
        return true;
    }

    //#########################################

    // Helper method to map MusicCollection entity to MusicCollectionResponseDto (MapToDto)
    private static MusicCollectionResponseDto MapToDto(MusicCollection item) 
    {
        return new MusicCollectionResponseDto
        {
            Id = item.Id,
            UserId = item.UserId,
            AlbumId = item.AlbumId,
            Format = item.Format,
            Condition = item.Condition,
            Status = item.Status,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }
}