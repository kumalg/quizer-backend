using quizer_backend.Data.Entities.QuizObject;

namespace quizer_backend.Data.Entities.LearningQuiz {
    public class LearningQuizQuestion : Entity<long> {
        public long LearningQuizId { get; set; }
        public long QuestionId { get; set; }
        public long Reoccurrences { get; set; }
        public long BadUserAnswers { get; set; }
        public long GoodUserAnswers { get; set; }

        public LearningQuiz LearningQuiz { get; set; }
        public Question Question { get; set; }
    }
}
