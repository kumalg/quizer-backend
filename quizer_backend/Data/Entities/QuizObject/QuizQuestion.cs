using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace quizer_backend.Data.Entities {
    public class QuizQuestion {
        public long Id { get; set; }
        public long QuizId { get; set; }
        public long CreationTime { get; set; }

        public Quiz Quiz { get; set; }
        public List<QuizQuestionVersion> Versions { get; set; }
        public List<QuizQuestionAnswer> Answers { get; set; }

        [NotMapped]
        public string Value { get; set; } // unnessesary
        //[NotMapped]
        //public long LastModifiedTime { get; set; } // unnessesary
    }

    public static class QuizQuestionExtensions {
        public static QuizQuestion FlatVersionProps(this QuizQuestion question, long? creationTime = null) {
            var version = creationTime == null
                ? question.Versions.OrderByDescending(i => i.CreationTime).FirstOrDefault()
                : question.Versions.Where(i => i.CreationTime <= creationTime).LastOrDefault();

            if (version != null) {
                question.Value = version.Value;
            }

            return question;
        }
    }
}
