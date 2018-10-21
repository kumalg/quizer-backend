using quizer_backend.Models;

namespace quizer_backend.Data.Entities {
    public class QuizAccess {
        public long Id { get; set; }
        public long QuizId { get; set; }
        public string UserId { get; set; }
        public QuizAccessEnum Access { get; set; } = QuizAccessEnum.None;

        public QuizItem Quiz { get; set; }
    }
}
