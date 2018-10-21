using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using quizer_backend.Data.Entities;

namespace quizer_backend.Models {
    public class QuizItemWIthAccess: QuizItem {
        [JsonConverter(typeof(StringEnumConverter))]
        public QuizAccessEnum Access { get; set; }
    }
}
