using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using quizer_backend.Data.Entities;

namespace quizer_backend.Data.Repository {
    public interface IGenericRepository<T> where T : BaseEntity {
        IQueryable<T> GetAll(bool asNoTracking = true);
        Task<T> GetById(object id);
        Task<EntityEntry> Create(T entity);
        EntityEntry Update(T entity);
        EntityEntry Delete(T entity);
        Task<EntityEntry> Delete(object id);
        Task<bool> Save();
    }
}
