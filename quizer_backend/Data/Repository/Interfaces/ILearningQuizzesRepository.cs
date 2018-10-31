using System.Linq;
using System.Threading.Tasks;
using quizer_backend.Data.Entities.LearningQuiz;

namespace quizer_backend.Data.Repository.Interfaces {
    public interface ILearningQuizzesRepository {
        Task<LearningQuiz> GetLearningQuizByIdAsync(string userId, long id, bool includeQuiz = false);
        Task<IQueryable<LearningQuiz>> GetAllLearningQuizzes(string userId, bool includeQuiz = false);
        IQueryable<LearningQuizQuestion> GetLearningQuizQuestionsReoccurrences(long learningQuizId);
        Task<LearningQuizQuestion> GetLearningQuizQuestions(long learningQuizId, long questionId);
        Task<bool?> IsLearningQuizFinished(string userId, long learningQuizId);
        Task<bool> FinishLearningQuiz(string userId, long learningQuizId);

        Task<bool> AddLearningQuizAsync(LearningQuiz learningQuiz);
    }
}
