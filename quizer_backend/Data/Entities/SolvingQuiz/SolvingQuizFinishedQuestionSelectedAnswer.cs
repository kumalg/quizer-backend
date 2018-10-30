using quizer_backend.Data.Entities.QuizObject;

namespace quizer_backend.Data.Entities.SolvingQuiz {
    public class SolvingQuizFinishedQuestionSelectedAnswer {
        public long Id { get; set; }
        public long FinishedQuestionId { get; set; }
        public long? QuizQuestionAnswerId { get; set; }

        public SolvingQuizFinishedQuestion FinishedQuestion { get; set; }
        public virtual QuizQuestionAnswer QuizQuestionAnswer { get; set; }
    }
}
