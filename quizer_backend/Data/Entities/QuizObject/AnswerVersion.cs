using System.ComponentModel.DataAnnotations;

namespace quizer_backend.Data.Entities.QuizObject {
    public class AnswerVersion {
        public long Id { get; set; }
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
