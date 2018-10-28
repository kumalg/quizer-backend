using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace quizer_backend.Data.Entities {
    public class QuizQuestionAnswer {
        public long Id { get; set; }
        public long QuizQuestionId { get; set; }
        public long CreationTime { get; set; }

        public QuizQuestion QuizQuestion { get; set; }
        public List<QuizQuestionAnswerVersion> Versions { get; set; }
        
        [NotMapped]
        public bool IsCorrect { get; set; } // unnessesary
        [NotMapped]
        public string Value { get; set; } // unnessesary
    }

    public static class QuizQuestionAnswerExtensions {
        public static QuizQuestionAnswer FlatVersionProps(this QuizQuestionAnswer answer, long? creationTime = null) {
            var version = creationTime == null
                ? answer.Versions.OrderByDescending(i => i.CreationTime).FirstOrDefault()
                : answer.Versions.Where(i => i.CreationTime <= creationTime).LastOrDefault();

            if (version != null) {
                answer.Value = version.Value;
                answer.IsCorrect = version.IsCorrect;
            }

            return answer;
        }
    }
}