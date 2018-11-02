using System.Linq;
using System.Threading.Tasks;

namespace quizer_backend.Data.SuperRepository {
    public interface IGenericRepository<TEntity> {
        IQueryable<TEntity> GetAll();
        Task<TEntity> GetById(long id);
        Task<bool> Create(TEntity entity);
        Task<bool> Update(long id, TEntity entity);
        Task<bool> Delete(long id);
    }
}
