using System.Linq.Expressions;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;

namespace MusicRUs.API.Tests.Fakes;

// Hand-written fake instead of Moq: keeps the users in a list, so the services run their real logic.
// The list holds the same User objects the services change, so a test can check them afterwards.
internal sealed class FakeUserRepository : IUserRepository
{
    private readonly List<User> _users = new();
    private int _nextId = 1;

    public void Seed(User user)
    {
        if (user.Id == 0) user.Id = _nextId;
        _nextId = Math.Max(_nextId, user.Id + 1);
        _users.Add(user);
    }

    // IReadRepository<User>
    public Task<User?> GetByIdAsync(int id)
        => Task.FromResult(_users.FirstOrDefault(u => u.Id == id));

    public Task<User?> GetByStringAsync(string value)
        => Task.FromResult(_users.FirstOrDefault(u => u.Username == value));

    public Task<IEnumerable<User>> GetAllAsync()
        => Task.FromResult<IEnumerable<User>>(_users.ToList());

    public Task<List<User>> GetByIdsAsync(IEnumerable<int> ids)
        => Task.FromResult(_users.Where(u => ids.Contains(u.Id)).ToList());

    public Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
        => Task.FromResult<(IEnumerable<User> Items, int TotalCount)>(
            (_users.Skip((page - 1) * pageSize).Take(pageSize).ToList(), _users.Count));

    public Task<IEnumerable<User>> SearchAsync(Expression<Func<User, bool>> predicate)
        => Task.FromResult<IEnumerable<User>>(_users.Where(predicate.Compile()).ToList());

    // IRepository<User>
    public Task AddAsync(User entity)
    {
        Seed(entity);
        return Task.CompletedTask;
    }

    public void Update(User entity) { } // Nothing to do: the list already holds the changed object

    public void Delete(User entity) => _users.Remove(entity);

    public Task SaveChangesAsync() => Task.CompletedTask;

    // IUserRepository
    public Task<User?> GetUserByUsernameAsync(string username)
        => Task.FromResult(_users.FirstOrDefault(u => u.Username == username));

    public Task<User?> GetUserByEmailAsync(string emailHash)
        => Task.FromResult(_users.FirstOrDefault(u => u.EmailHash == emailHash));

    public Task<User?> GetByGoogleIdAsync(string googleId)
        => Task.FromResult(_users.FirstOrDefault(u => u.GoogleId == googleId));

    public Task<IEnumerable<User>> SearchUsersByKeywordAsync(string keyword)
        => Task.FromResult<IEnumerable<User>>(
            _users.Where(u => u.Username.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList());

    public Task<bool> EmailExistsAsync(string emailHash)
        => Task.FromResult(_users.Any(u => u.EmailHash == emailHash));

    public Task<bool> UsernameExistsAsync(string username)
        => Task.FromResult(_users.Any(u => u.Username == username));
}
