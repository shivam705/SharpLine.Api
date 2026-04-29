using Microsoft.EntityFrameworkCore;
using SharpLine.Api.Data;
using SharpLine.Api.Repositories.Interface;

namespace SharpLine.Api.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;


        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }


        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();


        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);


        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }


        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }


        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }


        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
