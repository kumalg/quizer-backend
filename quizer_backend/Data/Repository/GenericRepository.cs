using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace quizer_backend.Data.Repository {
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class {
        private readonly QuizerContext _context;

        public GenericRepository(QuizerContext context) {
            _context = context;
        }

        public IQueryable<TEntity> GetAll() {
            return _context.Set<TEntity>().AsNoTracking();
        }

        public async Task<TEntity> GetById(object id) {
            return await _context.Set<TEntity>()
                .FindAsync(id);
        }

        public async Task<bool> Create(TEntity entity) {
            await _context.Set<TEntity>().AddAsync(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(object id, TEntity entity) {
            _context.Set<TEntity>().Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(object id) {
            var entity = await GetById(id);
            _context.Set<TEntity>().Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}