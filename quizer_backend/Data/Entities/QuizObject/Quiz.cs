using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Auth0.ManagementApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using quizer_backend.Models;

namespace quizer_backend.Data.Entities.QuizObject {
    public class Quiz : Entity<Guid> {
        [Required]
        public string Name { get; set; }
        public string OwnerId { get; set; }
        public long CreationTime { get; set; }
        public long LastModifiedTime { get; set; }
        public int? QuestionsInSolvingQuiz { get; set; }
        public int? MinutesInSolvingQuiz { get; set; }
        public bool IsPublic { get; set; }

        [JsonIgnore]
        public List<Question> Questions { get; set; }
        [JsonIgnore]
        public List<QuizAccess> Accesses { get; set; } = new List<QuizAccess>();

        [NotMapped]
        public long? MillisecondsInSolvingQuiz => MinutesInSolvingQuiz == null
            ? null
            : (long?)((long)MinutesInSolvingQuiz.Value * 60 * 1000);
        [NotMapped]
        public string OwnerNickName { get; set; }
        [NotMapped]
        public string OwnerPicture { get; set; }
        [NotMapped, JsonConverter(typeof(StringEnumConverter))]
        public QuizAccessEnum Access { get; set; }

        public Quiz IncludeOwner(User user) {
            OwnerPicture = user.Picture;
            OwnerNickName = user.NickName;
            return this;
        }
    }

    public static class QuizItemExtensions {
        public static Quiz IncludeAccess(this Quiz quiz, QuizAccessEnum access) {
            quiz.Access = access;
            return quiz;
        }
    }
}
