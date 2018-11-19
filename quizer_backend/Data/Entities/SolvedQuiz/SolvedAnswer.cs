using Newtonsoft.Json;
using quizer_backend.Data.Entities.QuizObject;

namespace quizer_backend.Data.Entities.SolvedQuiz
{
    public class SolvedAnswer : Entity<long> {
        public long SolvedQuestionId { get; set; }
        public long? AnswerId { get; set; }
        public bool IsSelected { get; set; }
        public bool IsCorrect { get; set; }

        [JsonIgnore] public SolvedQuestion SolvedQuestion { get; set; }
        [JsonIgnore] public Answer Answer { get; set; }
    }
}