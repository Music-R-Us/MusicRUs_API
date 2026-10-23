using Microsoft.EntityFrameworkCore;
using MusicRUs.API.Data;
using MusicRUs.API.Repositories.Interfaces;
using System.Linq.Expressions;

namespace MusicRUs.API.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        // Reference to database
        protected readonly AppDbContext _context;

        // Reference to DbSet 
        protected readonly DbSet<T> _dbSet;

        // Initialize the repository with the database context
        public Repository(AppDbContext context)
        {
            _context = context; 
            _dbSet = _context.Set<T>(); 
        }

        //##############################

        // Get row by ID
        public virtual async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        
        // Get all rows
        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync(); 

        // Search rows by predicate (predicate is a condition to filter the rows)
        public async Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();

        // Add new row
        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        // Update row
        public void Update(T entity) => _dbSet.Update(entity); 

        // Delete row
        public void Delete(T entity) => _dbSet.Remove(entity);

        // Save changes to database
        // (Uses _context specifically to save database changes, 
        // as _dbSet does not have a SaveChanges method)
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync(); 
    }
}