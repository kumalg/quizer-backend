using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Repository;

namespace quizer_backend.Data.SuperRepository {
    public class AnswersRepository : GenericRepository<Answer> {
        private readonly QuizerContext _context;

        public AnswersRepository(QuizerContext context) : base(context) {
            _context = context;
        }

        public IQueryable<Answer> GetAllByQuestionId(long questionId, long? maxVersionTime = null) {
            var query = _context.Answers
                .Where(q => q.QuestionId == questionId);

            if (maxVersionTime != null)
                query = query.Where(q => q.CreationTime <= maxVersionTime);

            return query;
        }

        public async Task<long> GetQuizId(long answerId) {
            return await GetAll()
                .Where(a => a.Id == answerId)
                .Include(a => a.Question)
                .Select(a => a.Question.QuizId)
                .SingleOrDefaultAsync();
        }
    }
}
