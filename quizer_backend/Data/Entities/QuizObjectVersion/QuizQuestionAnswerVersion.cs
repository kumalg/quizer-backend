namespace quizer_backend.Data.Entities {
    public class QuizQuestionAnswerVersion {
        public long Id { get; set; }
        public long QuizQuestionAnswerId { get; set; }
        public long CreationTime { get; set; }
        public string Value { get; set; }
        public bool IsCorrect { get; set; }

        public QuizQuestionAnswer QuizQuestionAnswer { get; set; }
    }
}
