using System.Collections.Generic;

namespace quizer_backend.Models {
    public class SolvingQuizQuestion {
        public long Id { get; set; }
        public string Value { get; set; }
        public List<SolvingQuizAnswer> Answers { get; set; }
    }

    public class SolvingQuizAnswer {
        public long Id { get; set; }
        public string Value { get; set; }
    }
}
