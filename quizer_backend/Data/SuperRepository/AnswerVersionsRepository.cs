using quizer_backend.Data.Entities.QuizObjectVersion;
using quizer_backend.Data.Repository;

namespace quizer_backend.Data.SuperRepository {
    public class AnswerVersionsRepository : GenericRepository<AnswerVersion> {
        private readonly QuizerContext _context;

        public AnswerVersionsRepository(QuizerContext context) : base(context) {
            _context = context;
        }
    }
}
