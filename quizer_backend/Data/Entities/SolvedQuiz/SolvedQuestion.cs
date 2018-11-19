using System.Collections.Generic;
using Newtonsoft.Json;
using quizer_backend.Data.Entities.QuizObject;

namespace quizer_backend.Data.Entities.SolvedQuiz
{
    public class SolvedQuestion : Entity<long> {
        public long SolvedQuizId { get; set; }
        public long? QuestionId { get; set; }
        public bool AnsweredCorrectly { get; set; }
        public List<SolvedAnswer> Answers { get; set; }

        [JsonIgnore] public SolvedQuiz SolvedQuiz { get; set; }
        [JsonIgnore] public Question Question { get; set; }
    }
}