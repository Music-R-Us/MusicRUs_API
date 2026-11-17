using Microsoft.EntityFrameworkCore;
using MusicRUs.API.Data;
using MusicRUs.API.Repositories.Interfaces;
using System.Linq.Expressions;

///////////////////////////////////////
/// SOLID Principles:
/// 
/// S: This class focuses on data access and manipulation for a single entity type T. 
/// O: The methods in the class are generic and can be extended/reused for new entity types without modifying this class.
/// L: Derived repositories (<X>Repository.cs) can replace Repository<T> without breaking the code. 
/// I: Methods are defined in the interface (IRepository<T>). Data model specific methods are kept in the derived classes (I<X>Repository.cs).
/// D: Higher level classes (services) depend on this repositories interface (IRepository<T>), instead of this concrete class. 
///////////////////////////////////////


namespace MusicRUs.API.Repositories
{
    public class Repository<T> : IRepository<T> where T : class // T is a generic type parameter
    {
        // Reference to database
        protected readonly DbContext _context;

        // Reference to DbSet 
        protected readonly DbSet<T> _dbSet;

        // Initialize the repository with the database context
        public Repository(DbContext context) //Dependency Injection of DbContext
        {
            _context = context; 
            _dbSet = _context.Set<T>(); 
        }

        //##############################

        // Get row by ID
        public virtual async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        
        // Get row by string value
        public virtual async Task<T?> GetByStringAsync(string value) 
            => await _dbSet.FirstOrDefaultAsync(e => EF.Property<string>(e, "Value") == value); 
        
        // Get all rows
        public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync(); 

        // Get entities by an array of IDs
        public async Task<List<T>> GetByIdsAsync(IEnumerable<int> ids) 
            => await _dbSet.Where(e => ids.Contains(EF.Property<int>(e, "Id"))).ToListAsync();

        // Get paginated entities
        /*public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int page, int pageSize) 
        {
            var totalCount = await _dbSet.CountAsync(); // Get total count for pagination
            var items = await _dbSet
                .Skip((page - 1) * pageSize)
                .Take(pageSize) 
                .ToListAsync(); // Get paginated items
            return (items, totalCount); // Return paginated items and total count
        }*/

        // Search rows by predicate (predicate is a condition to filter the rows)
        public virtual async Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();

        // Add new row
        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        // Update row
        public void Update(T entity) => _dbSet.Update(entity); 

        // Delete row
        public void Delete(T entity) => _dbSet.Remove(entity);

        // Save changes to database
        // (Uses '_context' specifically to save database changes, 
        // as '_dbSet' does not have a SaveChanges method)
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync(); 
    }
}