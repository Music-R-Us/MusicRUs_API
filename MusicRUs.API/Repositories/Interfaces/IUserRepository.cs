using MusicRUs.API.Models;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> GetUserByEmailAsync(string emailHash);
    Task<User?> GetByGoogleIdAsync(string googleId); // Get user by Google ID
    Task<IEnumerable<User>> SearchUsersByKeywordAsync(string keyword); // search users by username keyword
    Task<bool> EmailExistsAsync(string emailHash);
    Task<bool> UsernameExistsAsync(string username);
}