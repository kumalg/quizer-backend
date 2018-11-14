using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;

namespace quizer_backend.Data.Entities.QuizObject {
    public class Answer : Entity<long> {
        public long QuestionId { get; set; }
        public long CreationTime { get; set; }
        public bool IsDeleted { get; set; } = false;
        public long? DeletionTime { get; set; }

        [JsonIgnore]
        public Question Question { get; set; }
        [JsonIgnore]
        public List<AnswerVersion> Versions { get; set; } = new List<AnswerVersion>();
        
        [NotMapped]
        public bool IsCorrect { get; set; }
        [NotMapped]
        public string Value { get; set; }
    }

    public static class QuizQuestionAnswerExtensions {
        public static Answer FlatVersionProps(this Answer answer, long? versionMaxTime = null) {
            var version = versionMaxTime == null
                ? answer.Versions.OrderByDescending(i => i.CreationTime)
                                 .FirstOrDefault()
                : answer.Versions.Where(i => i.CreationTime <= versionMaxTime)
                                 .OrderByDescending(i => i.CreationTime)
                                 .FirstOrDefault();

            if (version != null) {
                answer.Value = version.Value;
                answer.IsCorrect = version.IsCorrect;
            }

            return answer;
        }
    }
}