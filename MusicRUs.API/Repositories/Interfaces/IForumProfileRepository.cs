using MusicRUs.API.Models;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Repositories.Interfaces;

public interface IForumProfileRepository : IRepository<ForumProfile>
{
    Task<ForumProfile?> GetFpByUserIdAsync(int userId);
    Task<ForumProfile?> GetWithFavoritesAsync(int userId);
    Task<IEnumerable<ForumProfile>> SearchPublicFpByKeywordAsync(string keyword); // searching profiles by displayname keywords
}