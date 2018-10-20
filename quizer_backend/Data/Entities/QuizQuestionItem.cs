using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace quizer_backend.Data.Entities {
    public class QuizQuestionItem {
        public long Id { get; set; }
        public long QuizId { get; set; }
        public long LastModifiedTime { get; set; }
        public string Value { get; set; }
        public List<QuizQuestionAnswerItem> Answers { get; set; }

        public QuizItem Quiz { get; set; }
    }
}