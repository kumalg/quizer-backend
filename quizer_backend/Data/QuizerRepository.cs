using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace quizer_backend.Data {
    public class QuizerRepository : IQuizerRepository {
        private readonly QuizerContext _context;

        public QuizerRepository(QuizerContext context) {
            _context = context;
        }

        public IEnumerable<QuizItem> GetAllMyQuizes(string userId) {
            return _context.QuizItems
                           .Where(q => q.OwnerId == userId)
                           .Include(b => b.QuizQuestions)
                           .ToList();
        }

        public QuizItem GetQuizById(long id) {
            return _context.QuizItems.FirstOrDefault(q => q.Id == id);
        }

        public bool SaveAll() {
            return _context.SaveChanges() > 0;
        }

        public void AddQuiz(QuizItem quiz) {
            long creationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            quiz.CreatedTime = creationTime;
            quiz.LastModifiedTime = creationTime;

            _context.QuizItems.Add(quiz);
            _context.SaveChanges();
        }

        public void AddQuizQuestion(QuizQuestionItem question) {
            long creationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            
            question.LastModifiedTime = creationTime;

            _context.QuizQuestionItems.Add(question);
            _context.SaveChanges();
        }

        public void AddQuizQuestionAnswer(QuizQuestionAnswerItem answer) {
            long creationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            answer.LastModifiedTime = creationTime;

            _context.QuizQuestionAnswerItems.Add(answer);
            _context.SaveChanges();
        }
    }
}
