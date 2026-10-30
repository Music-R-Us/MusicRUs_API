using MusicRUs.API.DTOs;

namespace MusicRUs.API.Services.Interfaces
{
    public interface IForumPostService
    {
        Task<ForumPostResponseDto> CreateForumPostAsync(CreateForumPostDto dto); // CREATE
        Task<ForumPostWithCommentsDto> GetForumPostByIdAsync(int id); // READ
        Task<IEnumerable<ForumPostResponseDto>> GetForumPostsByUserIdAsync(int userId); // READ
        Task<IEnumerable<ForumPostResponseDto>> GetForumPostsByTrackIdAsync(int trackId); // READ
        Task<IEnumerable<ForumPostResponseDto>> GetForumPostsByAlbumIdAsync(int albumId); // READ
        Task<IEnumerable<ForumPostResponseDto>> GetForumPostsByArtistIdAsync(int artistId); // READ
        Task<IEnumerable<ForumPostResponseDto>> GetAllForumPostsAsync(); // READ
        Task<IEnumerable<ForumPostResponseDto>> SearchForumPostsByKeywordAsync(string keyword); // READ
        Task<ForumPostResponseDto> UpdateForumPostAsync(int id, ForumPostUpdateDto dto); // UPDATE
        Task<bool> DeleteForumPostAsync(int id); // DELETE
    }
}