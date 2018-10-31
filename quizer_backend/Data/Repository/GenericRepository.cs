using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace quizer_backend.Data.Repository {
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class {
        private readonly QuizerContext _dbContext;

        public GenericRepository(QuizerContext dbContext) {
            _dbContext = dbContext;
        }

        public IQueryable<TEntity> GetAll() {
            return _dbContext.Set<TEntity>().AsNoTracking();
        }

        public async Task<TEntity> GetById(long id) {
            return await _dbContext.Set<TEntity>()
                .FindAsync(id);
        }

        public async Task<bool> Create(TEntity entity) {
            await _dbContext.Set<TEntity>().AddAsync(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(long id, TEntity entity) {
            _dbContext.Set<TEntity>().Update(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(long id) {
            var entity = await GetById(id);
            _dbContext.Set<TEntity>().Remove(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}