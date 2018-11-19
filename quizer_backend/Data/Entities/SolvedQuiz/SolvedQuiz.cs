using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using quizer_backend.Data.Entities.QuizObject;

namespace quizer_backend.Data.Entities.SolvedQuiz {
    public class SolvedQuiz : Entity<long> {
        public Guid QuizId { get; set; }
        public string UserId { get; set; }
        public long CreationTime { get; set; }
        public long FinishTime { get; set; }
        public long SolveTime { get; set; }
        public int CorrectCount { get; set; }
        public int BadCount { get; set; }
        public List<SolvedQuestion> Questions { get; set; }

        [JsonIgnore] public Quiz Quiz { get; set; }
    }
}
