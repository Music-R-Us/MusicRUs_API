using System.Linq.Expressions; 

namespace MusicRUs.API.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id); // Get entity by ID
        Task<IEnumerable<T>> GetAllAsync(); // Get all entities
        Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> predicate); // Search entities by predicate
        Task AddAsync(T entity); // Add a new entity
        void Update(T entity); // Update an existing entity
        void Delete(T entity); // Delete an entity
        Task SaveChangesAsync(); // Save changes to database
    }
}