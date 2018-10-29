using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using quizer_backend.Data.Entities;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Entities.QuizObjectVersion;

namespace quizer_backend.Data {
    public interface IQuizerRepository {
        Task<bool> SaveAllAsync();

        Task<Quiz> GetQuizByIdAsync(string userId, long id);
        Task<QuizQuestion> GetQuizQuestionByIdAsync(string userId, long id, long? maxTime = null, long? minTime = null);
        Task<QuizQuestionAnswer> GetQuizQuestionAnswerByIdAsync(string userId, long id, long? maxTime = null, long? minTime = null);
        Task<SolvingQuiz> GetSolvingQuizByIdAsync(string userId, long id);
        IQueryable<Quiz> GetAllQuizes(string userId);
        Task<IQueryable<QuizQuestion>> GetQuizQuestionsByQuizIdAsync(string userId, long id, long? maxTime = null, long? minTime = null);
        Task<IQueryable<QuizQuestionAnswer>> GetQuizQuestionAnswersByQuizQuestionIdAsync(string userId, long id, long? maxTime = null, long? minTime = null);
        Task<IQueryable<SolvingQuiz>> GetAllSolvingQuizes(string userId);
        Task<List<SolvingQuizFinishedQuestion>> GetSolvingQuizFinishedQuestions(string userId, long id, long? maxTime = null, long? minTime = null);

        Task<bool> AddQuizAsync(Quiz quiz);
        Task<bool> AddQuizQuestionWithVersionAsync(QuizQuestion question);
        Task<bool> AddQuizQuestionAnswerWithVersionAsync(QuizQuestionAnswer answer);
        Task<bool> AddQuizAccessAsync(QuizAccess quizAccess);
        Task<bool> AddQuizQuestionVersionAsync(QuizQuestionVersion questionVersion);
        Task<bool> AddQuizQuestionAnswerVersionAsync(QuizQuestionAnswerVersion answerVersion);
        Task<bool> AddSolvingQuizAsync(SolvingQuiz solvingQuiz);

        Task<bool> DeleteQuizByIdAsync(string userId, long id);
        Task<bool> DeleteQuizQuestionByIdAsync(string userId, long id);
        Task<bool> DeleteQuizQuestionAnswerByIdAsync(string userId, long id);
        Task<bool> DeleteSolvingQuizAsync(string userId, long id);
    }
}