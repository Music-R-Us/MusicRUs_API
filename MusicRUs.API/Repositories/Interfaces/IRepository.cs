using System.Linq.Expressions;

///////////////////////////////////////
/// SOLID Principles:
///
/// S: This interface only describes write operations for a single entity type T.
/// O: The interface extends IReadRepository<T> and is generic, so it can be reused for new entity types without modifying it.
/// L: Every implementation (Repository<T> and derived <X>Repository.cs) can replace IRepository<T>.
/// I: Write operations are kept here, while read operations are in IReadRepository<T>. Data model specific methods are kept in the derived interfaces (I<X>Repository.cs).
/// D: Services that need to change data depend on this interface (IRepository<T>), instead of the concrete Repository<T> class.
///////////////////////////////////////

namespace MusicRUs.API.Repositories.Interfaces
{
    public interface IRepository<T> : IReadRepository<T> where T : class
    {
        Task AddAsync(T entity); // Add a new entity
        void Update(T entity); // Update an existing entity
        void Delete(T entity); // Delete an entity
        Task SaveChangesAsync(); // Save changes to database
    }
}