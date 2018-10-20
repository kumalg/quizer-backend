using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace quizer_backend.Data.Entities {
    public class QuizItem {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string OwnerId { get; set; }
        //public List<string> CreatorsId { get; set; }
        public long CreatedTime { get; set; }
        public long LastModifiedTime { get; set; }
        public List<QuizQuestionItem> QuizQuestions { get; set; }
    }
}
