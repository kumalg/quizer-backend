using System.Linq;
using System.Threading.Tasks;
using quizer_backend.Data.Entities;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Entities.QuizObjectVersion;

namespace quizer_backend.Data.Repository.Interfaces {
    public interface IQuizerRepository {
        Task<bool> SaveAllAsync();

        Task<Quiz> GetQuizByIdAsync(string userId, long id);
        Task<Question> GetQuestionByIdAsync(string userId, long id, long? maxTime = null, long? minTime = null);
        Task<Answer> GetAnswerByIdAsync(string userId, long id, long? maxTime = null, long? minTime = null);
        IQueryable<Quiz> GetAllQuizzes(string userId);
        Task<IQueryable<Question>> GetQuestionsByQuizIdAsync(string userId, long id, long? maxTime = null, long? minTime = null);
        Task<IQueryable<Answer>> GetAnswersByQuestionIdAsync(string userId, long id, long? maxTime = null, long? minTime = null);

        Task<bool> AddQuizAsync(Quiz quiz);
        Task<bool> AddQuestionWithVersionAsync(Question question);
        Task<bool> AddAnswerWithVersionAsync(Answer answer);
        Task<bool> AddQuizAccessAsync(QuizAccess quizAccess);
        Task<bool> AddQuestionVersionAsync(QuestionVersion questionVersion);
        Task<bool> AddAnswerVersionAsync(AnswerVersion answerVersion);

        Task<bool> DeleteQuizByIdAsync(string userId, long id);
        Task<bool> DeleteQuestionByIdAsync(string userId, long id);
        Task<bool> DeleteAnswerByIdAsync(string userId, long id);
    }
}