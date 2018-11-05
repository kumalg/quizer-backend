using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;

namespace quizer_backend.Data.Entities.QuizObject {
    public class Question {
        public long Id { get; set; }
        public long QuizId { get; set; }
        public long CreationTime { get; set; }
        public bool IsDeleted { get; set; } = false;
        public long? DeletionTime { get; set; }

        [JsonIgnore]
        public Quiz Quiz { get; set; }
        [JsonIgnore]
        public List<QuestionVersion> Versions { get; set; }
        [JsonIgnore]
        public List<Answer> Answers { get; set; }

        [NotMapped]
        public string Value { get; set; }
    }

    public static class QuizQuestionExtensions {
        public static Question FlatVersionProps(this Question question, long? versionMaxTime = null) {
            var version = versionMaxTime == null
                ? question.Versions.OrderByDescending(i => i.CreationTime)
                                   .FirstOrDefault()
                : question.Versions.Where(i => i.CreationTime <= versionMaxTime)
                                   .OrderByDescending(i => i.CreationTime)
                                   .FirstOrDefault();

            if (version != null)
                question.Value = version.Value;

            return question;
        }
    }
}
