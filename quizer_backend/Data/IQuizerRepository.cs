using System.Collections.Generic;
using quizer_backend.Data.Entities;

namespace quizer_backend.Data {
    public interface IQuizerRepository {
        bool AnyQuizes();
        IEnumerable<QuizItem> GetAllQuizes(string userId);
        bool SaveAll();
        void AddQuiz(QuizItem quiz);
        void AddQuizQuestion(QuizQuestionItem question);
        IEnumerable<QuizQuestionItem> GetAllQuestions();
    }
}