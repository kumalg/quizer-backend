using System.Collections.Generic;
using System.Threading.Tasks;
using quizer_backend.Data.Entities;

namespace quizer_backend.Data {
    public interface IQuizerRepository {
        Task<bool> SaveAllAsync();

        Task<Quiz> GetQuizByIdAsync(string userId, long id);
        Task<QuizQuestion> GetQuizQuestionByIdAsync(string userId, long id, long? maxTime = null, long? minTime = null);
        Task<QuizQuestionAnswer> GetQuizQuestionAnswerByIdAsync(string userId, long id, long? maxTime = null, long? minTime = null);
        IEnumerable<Quiz> GetAllQuizes(string userId);
        Task<IEnumerable<QuizQuestion>> GetQuizQuestionsByQuizIdAsync(string userId, long id, long? maxTime = null, long? minTime = null);
        Task<IEnumerable<SolvingQuiz>> GetAllSolvingQuizes(string userId);
        Task<IEnumerable<QuizQuestionAnswer>> GetQuizQuestionAnswersByQuizQuestionIdAsync(string userId, long id, long? maxTime = null, long? minTime = null);
        Task<List<SolvingQuizFinishedQuestion>> GetSolvingQuizFinishedQuestions(string userId, long id, long? maxTime = null, long? minTime = null);

        Task<bool> AddQuizAsync(Quiz quiz);
        Task<bool> AddQuizQuestionWithVersionAsync(QuizQuestion question);
        Task<bool> AddQuizQuestionAnswerWithVersionAsync(QuizQuestionAnswer answer);
        Task<SolvingQuiz> GetSolvingQuizByIdAsync(string v, long id);
        Task<bool> AddQuizAccessAsync(QuizAccess quizAccess);
        //Task<bool> AddQuizVersionAsync(QuizVersion quizVersion);
        Task<bool> AddQuizQuestionVersionAsync(QuizQuestionVersion questionVersion);
        Task<bool> AddQuizQuestionAnswerVersionAsync(QuizQuestionAnswerVersion answerVersion);
        Task<bool> AddSolvingQuizAsync(SolvingQuiz solvingQuiz);

        Task<bool> DeleteQuizByIdAsync(string userId, long id);
        Task<bool> DeleteQuizQuestionByIdAsync(string userId, long id);
        Task<bool> DeleteQuizQuestionAnswerByIdAsync(string userId, long id);
        Task<bool> DeleteSolvingQuizAsync(string v, long id);
    }
}