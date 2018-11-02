using System.Linq;
using System.Threading.Tasks;
using quizer_backend.Data.Entities.LearningQuiz;

namespace quizer_backend.Data.Repository {
    public class LearningQuizQuestionsRepository : GenericRepository<LearningQuizQuestion> {
        private readonly QuizerContext _context;

        public LearningQuizQuestionsRepository(QuizerContext context) : base(context) {
            _context = context;
        }

        public IQueryable<LearningQuizQuestion> GetAllByLearningQuizId(long learningQuizId) {
            return GetAll().Where(a => a.LearningQuizId == learningQuizId);
        }

        public async Task<bool> CreateMany(IQueryable<LearningQuizQuestion> entity) {
            await _context.Set<LearningQuizQuestion>().AddRangeAsync(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
