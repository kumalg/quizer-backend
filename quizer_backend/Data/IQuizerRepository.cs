using System.Collections.Generic;
using quizer_backend.Data.Entities;

namespace quizer_backend.Data {
    public interface IQuizerRepository {
        bool SaveAll();

        void AddQuiz(QuizItem quiz);
        void AddQuizQuestion(QuizQuestionItem question);
        void AddQuizQuestionAnswer(QuizQuestionAnswerItem answer);

        IEnumerable<QuizItem> GetAllMyQuizes(string userId);

        QuizItem GetQuizById(long id);
    }
}