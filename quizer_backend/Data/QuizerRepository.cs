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

        public IEnumerable<QuizItem> GetAllQuizes(string userId) {
            return _context.QuizItems
                           .Where(q => q.OwnerId == userId)
                           .Include(b => b.QuizQuestions)
                           .ToList();
        }

        public IEnumerable<QuizItem> GetAllQuizes() {
            return _context.QuizItems
                           .Include(b => b.QuizQuestions)
                           .ToList();
        }

        public IEnumerable<QuizQuestionItem> GetAllQuestions() {
            return _context.QuizQuestionItems
                           .Include(b => b.Quiz)
                           .Include(b => b.Answers)
                           .ToList();
        }

        public bool AnyQuizes() {
            return _context.QuizItems.Any();
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
    }
}
