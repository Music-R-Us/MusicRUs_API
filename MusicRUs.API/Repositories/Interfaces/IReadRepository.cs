using System.Linq.Expressions;

///////////////////////////////////////
/// SOLID Principles:
///
/// S: This interface only describes READ operations for a single entity type T.
/// O: The interface is generic and can be reused for new entity types without modifying it.
/// L: Every implementation (Repository<T> and derived <X>Repository.cs) must follow this contract, (example: GetByIdAsync returns null if the row is not found.)
/// I: Read operations are separated from write operations (IRepository<T>). 
/// D: Services that only read data depend on this interface (IReadRepository<T>), instead of the concrete Repository<T> class.
///////////////////////////////////////

namespace MusicRUs.API.Repositories.Interfaces
{
    public interface IReadRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id); // Get entity by ID
        Task<IEnumerable<T>> GetAllAsync(); // Get all entities
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int page, int pageSize); // Get paginated entities
        Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> predicate); // Search entities by predicate
    }
}