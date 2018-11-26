using System.Linq;
using quizer_backend.Data.Entities.SolvedQuiz;

namespace quizer_backend.Data.Repository {
    public class SolvedQuizRepository : GenericRepository<SolvedQuiz> {
        private readonly QuizerContext _context;

        public SolvedQuizRepository(QuizerContext context) : base(context) {
            _context = context;
        }

        public SolvedQuiz AddQuestions(SolvedQuiz solvedQuiz, IQueryable<SolvedQuestion> questions) {
            solvedQuiz.Questions.AddRange(questions);
            return solvedQuiz;
        }

        public IQueryable<SolvedQuiz> GetAllByUserId(string userId) {
            return GetAll().Where(a => a.UserId == userId);
        }
    }
}
