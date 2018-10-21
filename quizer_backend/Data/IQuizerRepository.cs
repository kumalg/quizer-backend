using System.Collections.Generic;
using System.Threading.Tasks;
using quizer_backend.Data.Entities;

namespace quizer_backend.Data {
    public interface IQuizerRepository {
        Task<bool> SaveAllAsync();

        Task<QuizItem> GetQuizByIdAsync(string userId, long id);
        Task<QuizQuestionItem> GetQuizQuestionByIdAsync(string userId, long id);
        Task<QuizQuestionAnswerItem> GetQuizQuestionAnswerByIdAsync(string userId, long id);
        IEnumerable<QuizItem> GetAllQuizes(string userId);
        Task<IEnumerable<QuizQuestionItem>> GetQuizQuestionsByQuizIdAsync(string userId, long id);
        Task<IEnumerable<QuizQuestionAnswerItem>> GetQuizQuestionAnswersByQuizQuestionIdAsync(string userId, long id);

        Task<bool> AddQuizAsync(QuizItem quiz);
        Task<bool> AddQuizAccessAsync(QuizAccess quizAccess);
        Task<bool> AddQuizQuestionAsync(QuizQuestionItem question);
        Task<bool> AddQuizQuestionAnswerAsync(QuizQuestionAnswerItem answer);

        Task<bool> DeleteQuizByIdAsync(string userId, long id);
        Task<bool> DeleteQuizQuestionByIdAsync(string userId, long id);
        Task<bool> DeleteQuizQuestionAnswerByIdAsync(string userId, long id);
    }
}