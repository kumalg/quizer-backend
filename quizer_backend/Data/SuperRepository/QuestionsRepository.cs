using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities.QuizObject;

namespace quizer_backend.Data.SuperRepository {
    public class QuestionsRepository : GenericRepository<Question> {
        private readonly QuizerContext _context;

        public QuestionsRepository(QuizerContext context) : base(context) {
            _context = context;
        }

        public async Task<Question> GetById(long id, bool allowDeleted = false) {
            if (!allowDeleted) {
                return await GetAll()
                    .Where(a => a.Id == id)
                    .Where(a => !a.IsDeleted)
                    .SingleOrDefaultAsync();
            }

            return await base.GetById(id);
        }

        public IQueryable<Question> GetAllByQuizId(long quizId, long? maxVersionTime = null, bool allowDeleted = false) {
            var query = GetAll().Where(q => q.QuizId == quizId);

            if (!allowDeleted)
                query = query.Where(q => !q.IsDeleted);

            if (maxVersionTime != null)
                query = query.Where(q => q.CreationTime <= maxVersionTime);

            return query;
        }

        public async Task<bool> SilentDelete(long id) {
            var entity = await GetById(id);
            entity.IsDeleted = true;
            return await Update(id, entity);
        }
    }
}
