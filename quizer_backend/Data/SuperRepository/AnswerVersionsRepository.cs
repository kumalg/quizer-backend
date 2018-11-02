using quizer_backend.Data.Entities.QuizObjectVersion;

namespace quizer_backend.Data.SuperRepository {
    public class AnswerVersionsRepository : GenericRepository<AnswerVersion> {
        private readonly QuizerContext _context;

        public AnswerVersionsRepository(QuizerContext context) : base(context) {
            _context = context;
        }
    }
}
