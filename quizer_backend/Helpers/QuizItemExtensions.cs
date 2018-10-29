using quizer_backend.Data.Entities;
using quizer_backend.Data.Entities.QuizObject;

namespace quizer_backend.Helpers {
    public static class QuizItemExtensions {
        public static Quiz IncludeOwnerNickNameInQuiz(this Quiz quiz, string ownerNickName) {
            quiz.OwnerNickName = ownerNickName;
            return quiz;
        }
    }
}