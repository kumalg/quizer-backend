using quizer_backend.Data.Entities;

namespace quizer_backend.Helpers {
    public static class QuizItemExtensions {
        public static QuizItem IncludeOwnerNickNameInQuiz(this QuizItem quiz, string ownerNickName) {
            quiz.OwnerNickName = ownerNickName;
            return quiz;
        }
    }
}