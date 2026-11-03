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

    public async Task<MusicCollectionResponseDto?> AddToOwnedCollectionAsync(CreateOwnedMusicCollectionDto dto) // CREATE (Owned collection)
    {
        if (await _mcRepo.EntryExistAsync(dto.UserId, dto.AlbumId, dto.Format, CollectionStatus.Owned))
            return null!; // Entry already exists in the collection

        var item = new MusicCollection
        {
            UserId = dto.UserId,
            AlbumId = dto.AlbumId,
            Format = dto.Format,
            Condition = dto.Condition,
            Status = CollectionStatus.Owned
        };

        await _mcRepo.AddAsync(item);
        await _mcRepo.SaveChangesAsync();

        var saved = await _mcRepo.GetCollectionItemAsync(dto.UserId, item.Id);
        return MapToDto(saved!);
    }

    public async Task<MusicCollectionResponseDto?> AddToWantedCollectionAsync(CreateWantedMusicCollectionDto dto) // CREATE (Wanted collection)
    {
        if (await _mcRepo.EntryExistAsync(dto.UserId, dto.AlbumId, dto.Format, CollectionStatus.Wanted))
            return null!; // Entry already exists in the collection

        var item = new MusicCollection
        {
            UserId = dto.UserId,
            AlbumId = dto.AlbumId,
            Format = dto.Format,
            Condition = dto.Condition,
            Status = CollectionStatus.Wanted
        };

        await _mcRepo.AddAsync(item);
        await _mcRepo.SaveChangesAsync();

        var saved = await _mcRepo.GetCollectionItemAsync(dto.UserId, item.Id);
        return MapToDto(saved!);
    }

    public async Task<IEnumerable<MusicCollectionResponseDto>> GetOwnedCollectionByUserIdAsync(int userId) // READ Owned
    {
        var collection = await _mcRepo.GetCollectionByUserIdAsync(userId, CollectionStatus.Owned);
        return collection.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<MusicCollectionResponseDto>> GetWantedCollectionByUserIdAsync(int userId) // READ Wanted
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
    private static MusicCollectionResponseDto MapToDto(MusicCollection mc)
    {
        return new MusicCollectionResponseDto
        {
            Id = mc.Id,
            UserId = mc.UserId,
            AlbumId = mc.AlbumId,
            Format = mc.Format,
            Condition = mc.Condition,
            Status = mc.Status,
            CreatedAt = mc.CreatedAt,
            UpdatedAt = mc.UpdatedAt,
            Album = mc.Album is null ? null : new AlbumResponseDto
            {
                Id = mc.Album.Id,
                Title = mc.Album.Title,
                Artists = mc.Album.Artists
                    .Select(a => new ArtistIncludeDto {Id = a.Id, Name = a.Name, ImageUrl = a.ImageUrl})
                .ToList(),
                ReleaseDate = mc.Album.ReleaseDate
            }
        };
    }
}