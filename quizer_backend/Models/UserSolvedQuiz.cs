using System;
using System.Collections.Generic;

namespace quizer_backend.Models {
    public class UserSolvedQuiz {
        public Guid QuizId { get; set; }
        public long CreatedTime { get; set; }
        public long SolvingTime { get; set; }
        public long? MaxSolvingTime { get; set; }
        public List<UserSolvedQuizQuestion> UserSolvedQuestions { get; set; }
    }

    public class UserSolvedQuizQuestion {
        public long QuestionId { get; set; }
        public List<long> SelectedAnswerIds { get; set; }
    }
}
