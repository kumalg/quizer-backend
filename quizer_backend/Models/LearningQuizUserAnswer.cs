using System.Collections.Generic;

namespace quizer_backend.Models {
    public class LearningQuizUserAnswer {
        public long QuizQuestionId { get; set; }
        public List<long> SelectedAnswers { get; set; }
    }
}
