using quizer_backend.Data.Entities.QuizObject;

namespace quizer_backend.Data.Entities.LearningQuiz {
    public class LearningQuizQuestionReoccurrences {
        public long Id { get; set; }
        public long LearningQuizId { get; set; }
        public long QuizQuestionId { get; set; }
        public long Reoccurrences { get; set; }

        public LearningQuiz LearningQuiz { get; set; }
        public QuizQuestion QuizQuestion { get; set; }
    }
}
