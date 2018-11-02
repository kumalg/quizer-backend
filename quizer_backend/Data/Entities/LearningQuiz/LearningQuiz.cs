using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Helpers;

namespace quizer_backend.Data.Entities.LearningQuiz {
    public class LearningQuiz {
        public long Id { get; set; }
        public long? QuizId { get; set; }
        public string UserId { get; set; }
        public long CreationTime { get; set; }
        public bool IsFinished { get; set; } = false;
        public long? FinishedTime { get; set; }
        public long LearningTime { get; set; } = 0;

        public long NumberOfQuestions { get; set; }
        public long NumberOfLearnedQuestions { get; set; } = 0;
        public long NumberOfCorrectAnswers { get; set; } = 0;
        public long NumberOfBadAnswers { get; set; } = 0;

        public virtual Quiz Quiz { get; set; }
        public List<LearningQuizQuestion> LearningQuizQuestions { get; set; }

        [NotMapped]
        public double FinishedRatio => IsFinished
            ? 0
            : (double)NumberOfLearnedQuestions / NumberOfQuestions;
    }

    public static class LearningQuizExtensions {
        public static LearningQuiz IncludeOwnerNickNameInQuiz(this LearningQuiz learningQuiz, string nickname) {
            learningQuiz.Quiz.IncludeOwnerNickName(nickname);
            return learningQuiz;
        }
    }
}
