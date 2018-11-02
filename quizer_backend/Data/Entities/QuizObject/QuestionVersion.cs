using System.ComponentModel.DataAnnotations;

namespace quizer_backend.Data.Entities.QuizObject {
    public class QuestionVersion {
        public long Id { get; set; }
        [Required]
        public long QuizQuestionId { get; set; }
        [Required]
        public long CreationTime { get; set; }
        [Required]
        public string Value { get; set; }

        public Question Question { get; set; }
    }
}
