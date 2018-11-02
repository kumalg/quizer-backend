using System.Linq;
using System.Threading.Tasks;
using quizer_backend.Data.Entities.LearningQuiz;

namespace quizer_backend.Data.Repository {
    public class LearningQuizzesRepository : GenericRepository<LearningQuiz> {
        private readonly QuizerContext _context;

        public LearningQuizzesRepository(QuizerContext context) : base(context) {
            _context = context;
        }

        public IQueryable<LearningQuiz> GetAllByUserId(string userId) {
            return GetAll().Where(a => a.UserId == userId);
        }

        public async Task<bool> SetAsFinished(long id, long finishedTime) {
            var quiz = await GetById(id);
            quiz.FinishedTime = finishedTime;
            quiz.IsFinished = true;
            return await Update(id, quiz);
        }
    }
}
