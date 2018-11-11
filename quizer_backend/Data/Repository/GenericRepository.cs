using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace quizer_backend.Data.Repository {
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class {
        private readonly QuizerContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(QuizerContext context) {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public IQueryable<TEntity> GetAll(bool asNoTracking = true) {
            return asNoTracking ? _dbSet.AsNoTracking() : _context.Set<TEntity>();
        }

        public async Task<TEntity> GetById(object id) {
            return await _dbSet.FindAsync(id);
        }

        public async Task<bool> Create(TEntity entity) {
            await _dbSet.AddAsync(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(object id, TEntity entity) {
            _dbSet.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(object id) {
            var entity = await GetById(id);
            _dbSet.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Save() {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}