using System.Collections.Generic;
using quizer_backend.Data.Entities.QuizObject;

namespace quizer_backend.Data.Entities.SolvingQuiz {
    public class SolvingQuizFinishedQuestion {
        public long Id { get; set; }
        public long SolvingQuizId { get; set; }
        public long? QuizQuestionId { get; set; }
        public bool CorrectlyAnswered { get; set; }

        public Entities.SolvingQuiz.SolvingQuiz SolvingQuiz { get; set; }
        public virtual QuizQuestion QuizQuestion { get; set; }
        public List<SolvingQuizFinishedQuestionSelectedAnswer> SelectedAnswers { get; set; }
    }
}
