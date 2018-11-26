using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using quizer_backend.Data.Entities;

namespace quizer_backend.Data.Repository {
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity {
        private readonly QuizerContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(QuizerContext context) {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual IQueryable<T> GetAll(bool asNoTracking = true) {
            return asNoTracking ? _dbSet.AsNoTracking() : _context.Set<T>();
        }

        public virtual async Task<T> GetById(object id) {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<EntityEntry> Create(T entity) {
            return await _dbSet.AddAsync(entity);
        }

        public virtual EntityEntry Update(T entity) {
            return _dbSet.Update(entity);
        }

        public virtual EntityEntry Delete(T entity) {
            return _dbSet.Remove(entity);
        }

        public virtual void DeleteRange(IQueryable<T> entities) {
            _dbSet.RemoveRange(entities);
        }

        public virtual async Task<EntityEntry> Delete(object id) {
            var entity = await GetById(id);
            return _dbSet.Remove(entity);
        }

        public virtual async Task<bool> Save() {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}