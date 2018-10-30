using System.Linq;
using System.Threading.Tasks;
using quizer_backend.Data.Entities.LearningQuiz;

namespace quizer_backend.Data.Repository.Interfaces {
    public interface ILearningQuizzesRepository {
        Task<LearningQuiz> GetLearningQuizByIdAsync(string userId, long id, bool includeQuiz = false);
        Task<IQueryable<LearningQuiz>> GetAllLearningQuizes(string userId, bool includeQuiz = false);
        IQueryable<LearningQuizQuestionReoccurrences> GetLearningQuizQuestionsReoccurrences(long learningQuizId);
        Task<LearningQuizQuestionReoccurrences> GetLearningQuizQuestionReoccurrences(long learningQuizId, long questionId);
        Task<bool?> IsLearningQuizFinished(string userId, long learningQuizId);
        Task<bool> FinishLearningQuiz(string userId, long learningQuizId);

        Task<bool> AddLearningQuizAsync(LearningQuiz learningQuiz);
    }
}
