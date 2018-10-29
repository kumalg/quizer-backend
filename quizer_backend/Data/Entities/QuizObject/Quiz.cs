using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using quizer_backend.Models;

namespace quizer_backend.Data.Entities.QuizObject {
    public class Quiz {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string OwnerId { get; set; }
        public long CreationTime { get; set; }
        public long LastModifiedTime { get; set; }
        
        [JsonIgnore]
        public List<QuizQuestion> QuizQuestions { get; set; }
        [JsonIgnore]
        public List<QuizAccess> Creators { get; set; }

        [NotMapped]
        public string OwnerNickName { get; set; }
        [NotMapped, JsonConverter(typeof(StringEnumConverter))]
        public QuizAccessEnum Access { get; set; }
    }

    public static class QuizItemExtensions {
        public static Quiz IncludeAccess(this Quiz quiz, QuizAccessEnum access) {
            quiz.Access = access;
            return quiz;
        }
    }
}
