using quizer_backend.Data.Entities.QuizObject;

namespace quizer_backend.Data.Entities.QuizObjectVersion {
    public class QuizQuestionVersion {
        public long Id { get; set; }
        public long QuizQuestionId { get; set; }
        public long CreationTime { get; set; }
        public string Value { get; set; }

        public QuizQuestion QuizQuestion { get; set; }
    }
}
