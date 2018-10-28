using quizer_backend.Helpers;
using System.Collections.Generic;

namespace quizer_backend.Data.Entities {
    public class SolvingQuiz {
        public long Id { get; set; }
        public long? QuizId { get; set; }
        public string UserId { get; set; }
        public long CreationTime { get; set; }

        public virtual Quiz Quiz { get; set; }
        public List<SolvingQuizFinishedQuestion> FinishedQuestions { get; set; }
    }

    public static class SolvingQuizExtensions {
        public static SolvingQuiz IncludeOwnerNickNameInQuiz(this SolvingQuiz solvingQuiz, string nickname) {
            solvingQuiz.Quiz.IncludeOwnerNickNameInQuiz(nickname);
            return solvingQuiz;
        }
    }
}
