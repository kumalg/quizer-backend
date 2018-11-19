using System;
using Newtonsoft.Json;

namespace quizer_backend.Data.Entities.QuizObject {
    public class QuizSessions : Entity<Guid> {
        public long NumberOfSolveSessions { get; set; } = 0;
        public long NumberOfLearnSessions { get; set; } = 0;

        [JsonIgnore]
        public Quiz Quiz { get; set; }
    }
}
