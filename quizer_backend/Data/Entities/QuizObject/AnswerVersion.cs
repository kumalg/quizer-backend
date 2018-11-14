using System.ComponentModel.DataAnnotations;

namespace quizer_backend.Data.Entities.QuizObject {
    public class AnswerVersion : Entity<long> {
        [Required]
        public long QuizQuestionAnswerId { get; set; }
        [Required]
        public long CreationTime { get; set; }
        [Required]
        public string Value { get; set; }
        public bool IsCorrect { get; set; }

        public Answer Answer { get; set; }
    }
}
