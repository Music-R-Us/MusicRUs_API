using MusicRUs.API.DTOs;

namespace MusicRUs.API.Services.Interfaces;

public interface IForumProfileService
{
    Task<ForumProfileResponseDto?> CreateFpAsync (CreateForumProfileDto dto); // CREATE (after user creation)
    Task<ForumProfileResponseDto?> GetFpByUserIdAsync(int userId, int? viewerId); // READ
    Task<ForumProfileResponseDto?> GetFpByCurrentUserIdAsync(int userId); // READ
    Task<IEnumerable<ForumProfileResponseDto>> SearchPublicFpByKeywordAsync(string keyword, int? viewerId); // READ
    Task<ForumProfileResponseDto?> UpdateFpAsync(int userId, ForumProfileUpdatedDto dto); // UPDATE
}