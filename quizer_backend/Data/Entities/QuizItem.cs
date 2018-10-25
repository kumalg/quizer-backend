using quizer_backend.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quizer_backend.Data.Entities {
    public class QuizItem {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string OwnerId { get; set; }
        public long CreatedTime { get; set; }
        public long LastModifiedTime { get; set; }

        public List<QuizQuestionItem> QuizQuestions { get; set; }        
        public List<QuizAccess> Creators { get; set; }

        [NotMapped]
        public string OwnerNickName { get; set; }
    }
    public static class QuizItemExtensions {
        public static QuizItemWIthAccess ToQuizItemWIthAccess(this QuizItem quiz, QuizAccessEnum access) {
            return new QuizItemWIthAccess {
                Id = quiz.Id,
                Name = quiz.Name,
                OwnerId = quiz.OwnerId,
                CreatedTime = quiz.CreatedTime,
                LastModifiedTime = quiz.LastModifiedTime,
                QuizQuestions = quiz.QuizQuestions,
                Creators = quiz.Creators,
                Access = access
            };
        }
    }
}
