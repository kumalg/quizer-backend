using System.ComponentModel.DataAnnotations.Schema;

namespace quizer_backend.Data.Entities {
    public class QuizQuestionAnswerItem {
        public long Id { get; set; }
        public long QuizQuestionId { get; set; }
        public long LastModifiedTime { get; set; }
        public bool IsCorrect { get; set; }
        public string Value { get; set; }
        
        public QuizQuestionItem QuizQuestion { get; set; }
    }
}