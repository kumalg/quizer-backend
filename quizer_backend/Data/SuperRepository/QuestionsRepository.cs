using System.Linq;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Repository;

namespace quizer_backend.Data.SuperRepository {
    public class QuestionsRepository : GenericRepository<Question> {
        private readonly QuizerContext _context;

        public QuestionsRepository(QuizerContext context) : base(context) {
            _context = context;
        }

        public IQueryable<Question> GetAllByQuizId(long quizId, long? maxVersionTime = null) {
            var query = _context.Questions
                .Where(q => q.QuizId == quizId);

            if (maxVersionTime != null)
                query = query.Where(q => q.CreationTime <= maxVersionTime);

            return query;
        }
    }
}
