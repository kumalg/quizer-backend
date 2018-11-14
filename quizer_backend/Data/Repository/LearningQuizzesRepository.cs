using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using quizer_backend.Data.Entities.LearningQuiz;

namespace quizer_backend.Data.Repository {
    public class LearningQuizzesRepository : GenericRepository<LearningQuiz> {
        private readonly QuizerContext _context;

        public LearningQuizzesRepository(QuizerContext context) : base(context) {
            _context = context;
        }

        public LearningQuiz AddQuestions(LearningQuiz learningQuiz, IQueryable<LearningQuizQuestion> questions) {
            learningQuiz.LearningQuizQuestions.AddRange(questions);
            return learningQuiz;
        }

        public IQueryable<LearningQuiz> GetAllByUserId(string userId) {
            return GetAll().Where(a => a.UserId == userId);
        }

        //public async Task<EntityEntry> SetAsFinished(long id, long finishedTime) {
        //    var quiz = await GetById(id);
        //    quiz.FinishedTime = finishedTime;
        //    quiz.IsFinished = true;
        //    return Update(quiz);
        //}
    }
}
