using MusicRUs.API.DTOs;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;
using MusicRUs.API.Services.Interfaces;

namespace MusicRUs.API.Services;

public class ForumProfileService : IForumProfileService
{
    private readonly IForumProfileRepository _fpRepo;

    public ForumProfileService(IForumProfileRepository fpRepository)
    {
        _fpRepo = fpRepository;
    }

    public async Task<ForumProfileResponseDto?> CreateFpAsync(CreateForumProfileDto dto) // CREATE
    {
        var existingProfile = await _fpRepo.GetFpByUserIdAsync(dto.UserId);
        if (existingProfile != null)
            return null;
        
        var fp = new ForumProfile
        {
            UserId = dto.UserId
        };

        await _fpRepo.AddAsync(fp);
        await _fpRepo.SaveChangesAsync();
        return MapToDto(fp);
    }

    public async Task<ForumProfileResponseDto?> GetFpByUserIdAsync(int userId) // READ
    {
        var fp = await _fpRepo.GetFpByUserIdAsync(userId);
        if (fp == null)
            return null;
        
        return MapToDto(fp);
    }

    // Search profiles by displayname keyword
    public async Task<IEnumerable<ForumProfileResponseDto>> SearchFpByKeywordAsync(string keyword) // READ
    {
        var fp = await _fpRepo.SearchFpByKeywordAsync(keyword);
        return fp.Select(MapToDto);
    }

    public async Task<ForumProfileResponseDto?> UpdateFpAsync(int userId, ForumProfileUpdatedDto dto)
    {
        var fp = await _fpRepo.GetFpByUserIdAsync(userId);
        if(fp == null)
            return null;
        
        fp.DisplayName = dto.DisplayName ?? fp.DisplayName; // Update fields if provided, else keep existing values
        fp.Bio = dto.Bio ?? fp.Bio;
        fp.AvatarUrl = dto.AvatarUrl ?? fp.AvatarUrl;
        fp.Location = dto.Location ?? fp.Location;
        fp.WebsiteUrl = dto.WebsiteUrl ?? fp.WebsiteUrl;
        fp.PrimaryColor = dto.PrimaryColor?.ToUpperInvariant() ?? fp.PrimaryColor;
        fp.AccentColor = dto.AccentColor?.ToUpperInvariant() ?? fp.AccentColor;
        fp.IsPublic = dto.IsPublic ?? fp.IsPublic;
        fp.UpdatedAt = DateTime.UtcNow;

        _fpRepo.Update(fp);
        await _fpRepo.SaveChangesAsync();
        return MapToDto(fp);
    }

//#########################################

    // Helper method to map ForumProfile entity to ForumProfileResponseDto (MapToDto)
    private static ForumProfileResponseDto MapToDto(ForumProfile fp) => new() // Helper method to map ForumProfile to ForumProfileResponseDto
    {
        Id = fp.Id,
        UserId = fp.UserId,
        DisplayName = fp.DisplayName,
        Bio = fp.Bio,
        AvatarUrl = fp.AvatarUrl,
        Location = fp.Location,
        WebsiteUrl = fp.WebsiteUrl,
        PrimaryColor = fp.PrimaryColor,
        AccentColor = fp.AccentColor,
        IsPublic = fp.IsPublic,
        CreatedAt = fp.CreatedAt,
        UpdatedAt = fp.UpdatedAt
    };
}