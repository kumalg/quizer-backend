﻿//using System.Collections.Generic;
//using Newtonsoft.Json;
//using quizer_backend.Data.Entities.QuizObject;
//using quizer_backend.Helpers;

//namespace quizer_backend.Data.Entities.SolvingQuiz {
//    public class SolvingQuiz {
//        public long Id { get; set; }
//        public long? QuizId { get; set; }
//        public string UserId { get; set; }
//        public long CreationTime { get; set; }
//        public bool IsFinished { get; set; } = false;
        
//        public virtual Quiz Quiz { get; set; }
//        [JsonIgnore]
//        public List<SolvingQuizFinishedQuestion> FinishedQuestions { get; set; }
//    }

//    public static class SolvingQuizExtensions {
//        public static SolvingQuiz IncludeOwnerNickName(this SolvingQuiz solvingQuiz, string nickname) {
//            solvingQuiz.Quiz.IncludeOwnerNickName(nickname);
//            return solvingQuiz;
//        }
//    }
//}
