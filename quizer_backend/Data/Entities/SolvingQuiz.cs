using Newtonsoft.Json;
using quizer_backend.Helpers;
using System.Collections.Generic;
using quizer_backend.Data.Entities.QuizObject;

namespace quizer_backend.Data.Entities {
    public class SolvingQuiz {
        public long Id { get; set; }
        public long? QuizId { get; set; }
        public string UserId { get; set; }
        public long CreationTime { get; set; }
        public bool IsFinished { get; set; } = false;
        
        public virtual Quiz Quiz { get; set; }
        [JsonIgnore]
        public List<SolvingQuizFinishedQuestion> FinishedQuestions { get; set; }
    }

    public static class SolvingQuizExtensions {
        public static SolvingQuiz IncludeOwnerNickNameInQuiz(this SolvingQuiz solvingQuiz, string nickname) {
            solvingQuiz.Quiz.IncludeOwnerNickNameInQuiz(nickname);
            return solvingQuiz;
        }
    }
}
