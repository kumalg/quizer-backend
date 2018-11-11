using System.Linq;
using System.Threading.Tasks;

namespace quizer_backend.Data.Repository {
    public interface IGenericRepository<TEntity> {
        IQueryable<TEntity> GetAll(bool asNoTracking = true);
        Task<TEntity> GetById(object id);
        Task<bool> Create(TEntity entity);
        Task<bool> Update(object id, TEntity entity);
        Task<bool> Delete(object id);
        Task<bool> Save();
    }
}
