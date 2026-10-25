using Microsoft.EntityFrameworkCore;
using MusicRUs.API.Data;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;

namespace MusicRUs.API.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(DbContext dbContext) : base(dbContext) { } 

    public async Task<User?> GetUserByUsernameAsync(string username)
        => await _dbSet.FirstOrDefaultAsync(u => u.Username == username);

    public async Task<User?> GetUserByEmailAsync(string email)
        => await _dbSet.FirstOrDefaultAsync(u => u.Email == email); 
    
    public async Task<bool> EmailExistsAsync(string email)
        => await _dbSet.AnyAsync(u => u.Email == email);

    public async Task<bool> UsernameExistsAsync(string username)
        => await _dbSet.AnyAsync(u => u.Username == username);

    public async Task<IEnumerable<User>> SearchUsersByKeywordAsync(string keyword)
        // Use ILike for case-insensitive search in PostgreSQL
        => await SearchAsync(u => EF.Functions.ILike(u.Username, $"%{keyword}%")); 
}