using MusicRUs.API.Models;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Repositories.Interfaces;

public interface IForumPostRepository : IRepository<ForumPost>
{
    Task<IEnumerable<ForumPost?>> GetForumPostsByUserIdAsync(int userId);
    Task<IEnumerable<ForumPost?>> GetForumPostsByTrackIdAsync(int trackId);
    Task<IEnumerable<ForumPost?>> GetForumPostsByAlbumIdAsync(int albumId);
    Task<IEnumerable<ForumPost?>> GetForumPostsByArtistIdAsync(int artistId);
    Task<IEnumerable<ForumPost?>> GetForumPostsByGenreIdAsync(int genreId);
}