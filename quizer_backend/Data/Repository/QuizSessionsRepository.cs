using quizer_backend.Data.Entities.QuizObject;

namespace quizer_backend.Data.Repository {
    public class QuizSessionsRepository : GenericRepository<QuizSessions> {
        private readonly QuizerContext _context;

        public QuizSessionsRepository(QuizerContext context) : base(context) {
            _context = context;
        }
    }
}
