using MusicRUs.API.DTOs;

namespace MusicRUs.API.Services.Interfaces
{
    public interface IForumPostService
    {
        Task<ForumPostResponseDto> CreateForumPostAsync(int userId, CreateForumPostDto dto); // CREATE
        Task<ForumPostWithCommentsDto> GetForumPostByIdAsync(int id, int? viewerId); // READ
        Task<IEnumerable<ForumPostResponseDto>> GetForumPostsByUserIdAsync(int userId, int? viewerId); // READ
        Task<IEnumerable<ForumPostResponseDto>> GetForumPostsByTrackIdAsync(int trackId, int? viewerId); // READ
        Task<IEnumerable<ForumPostResponseDto>> GetForumPostsByAlbumIdAsync(int albumId, int? viewerId); // READ
        Task<IEnumerable<ForumPostResponseDto>> GetForumPostsByArtistIdAsync(int artistId, int? viewerId); // READ
        Task<IEnumerable<ForumPostResponseDto>> GetForumPostsByGenreIdAsync(int genreId, int? viewerId); // READ
        Task<IEnumerable<ForumPostResponseDto>> GetAllForumPostsAsync(int? viewerId); // READ
        Task<IEnumerable<ForumPostResponseDto>> SearchForumPostsByKeywordAsync(string keyword, int? viewerId); // READ
        Task<ForumPostResponseDto> UpdateForumPostAsync(int id, int userId,ForumPostUpdateDto dto); // UPDATE
        Task DeleteForumPostAsync(int id, int userId); // DELETE
    }
}