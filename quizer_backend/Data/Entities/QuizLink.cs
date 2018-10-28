using quizer_backend.Models;

namespace quizer_backend.Data.Entities {
    public class QuizLink {
        public long Id { get; set; }
        public long QuizId { get; set; }
        public string Link { get; set; }
        public QuizLinkAccessEnum Access { get; set; }

        public Quiz Quiz { get; set; }
    }
}
