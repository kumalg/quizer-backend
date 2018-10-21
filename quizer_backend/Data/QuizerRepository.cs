using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities;
using quizer_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace quizer_backend.Data {
    public class QuizerRepository : IQuizerRepository {
        private readonly QuizerContext _context;

        public QuizerRepository(QuizerContext context) {
            _context = context;
        }

        public async Task<bool> SaveAllAsync() {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<QuizAccess> UserAccessToQuizAsync(string userId, long quizId) {
            return await _context.QuizAccessItems
                                  .Where(i =>
                                       i.QuizId == quizId &&
                                       i.UserId == userId &&
                                       i.Access != QuizAccessEnum.None
                                   )
                                  .FirstOrDefaultAsync();
        }


        // GETOS
                
        public async Task<QuizItem> GetQuizByIdAsync(string userId, long id) {
            var access = await UserAccessToQuizAsync(userId, id);

            if (access.Access == QuizAccessEnum.None)
                return null;

            return await _context.QuizItems.FirstOrDefaultAsync(i => i.Id == id);
            //return _context.QuizAccessItems
            //               .Where(i =>
            //                    i.QuizId == id &&
            //                    i.UserId == userId &&
            //                    i.Access != QuizAccessEnum.None
            //                )
            //               .Include(i => i.Quiz)
            //               .FirstOrDefault()
            //               ?.Quiz;
        }

        public async Task<QuizQuestionItem> GetQuizQuestionByIdAsync(string userId, long id) {
            var question = await _context.QuizQuestionItems
                                         .Where(i => i.Id == id)
                                         .FirstOrDefaultAsync();

            if (question == null || !await _context.QuizAccessItems.AnyAsync(i => i.QuizId == question.QuizId && i.UserId == userId))
                return null;

            return question;
        }
        
        public async Task<QuizQuestionAnswerItem> GetQuizQuestionAnswerByIdAsync(string userId, long id) {
            var answer = await _context.QuizQuestionAnswerItems
                                       .Where(i => i.Id == id)
                                       .Include(i => i.QuizQuestion)
                                       .FirstOrDefaultAsync();

            if (answer == null || !await _context.QuizAccessItems.AnyAsync(i => i.QuizId == answer.QuizQuestion.QuizId && i.UserId == userId))
                return null;

            return answer;
        }

        public IEnumerable<QuizItem> GetAllQuizes(string userId) {
            return _context.QuizAccessItems
                           .Where(i => i.UserId == userId)
                           .Include(b => b.Quiz)
                           .Select(p => p.Quiz.ToQuizItemWIthAccess(p.Access));
        }

        public async Task<IEnumerable<QuizQuestionItem>> GetQuizQuestionsByQuizIdAsync(string userId, long id) {
            var hasAccess = await _context.QuizAccessItems
                                          .FirstOrDefaultAsync(i => 
                                              i.UserId == userId && 
                                              i.QuizId == id && 
                                              i.Access != QuizAccessEnum.None
                                          );

            if (hasAccess == null)
                return null;

            return _context.QuizQuestionItems.Where(i => i.QuizId == id);
        }

        public async Task<IEnumerable<QuizQuestionAnswerItem>> GetQuizQuestionAnswersByQuizQuestionIdAsync(string userId, long id) {
            var question = await _context.QuizQuestionItems
                                          .Where(i => i.Id == id)
                                          .Include(i => i.Answers)
                                          .FirstOrDefaultAsync();

            if (question == null || !(await _context.QuizAccessItems.AnyAsync(i => i.QuizId == question.QuizId && i.UserId == userId)))
                return null;

            return question.Answers;
        }


        // ADDOS

        public async Task<bool> AddQuizAsync(QuizItem quiz) {
            long creationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            quiz.CreatedTime = creationTime;
            quiz.LastModifiedTime = creationTime;

            await _context.QuizItems.AddAsync(quiz);
            return await SaveAllAsync();
        }

        public async Task<bool> AddQuizAccessAsync(QuizAccess access) {
            await _context.QuizAccessItems.AddAsync(access);
            return await SaveAllAsync();
        }

        public async Task<bool> AddQuizQuestionAsync(QuizQuestionItem question) {
            long creationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            question.LastModifiedTime = creationTime;

            await _context.QuizQuestionItems.AddAsync(question);
            return await SaveAllAsync();
        }

        public async Task<bool> AddQuizQuestionAnswerAsync(QuizQuestionAnswerItem answer) {
            long creationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            answer.LastModifiedTime = creationTime;

            await _context.QuizQuestionAnswerItems.AddAsync(answer);
            return await SaveAllAsync();
        }


        // DELETOS

        public async Task<bool> DeleteQuizByIdAsync(string userId, long id) {
            var quiz = await GetQuizByIdAsync(userId, id);
            if (quiz == null) return false;
            _context.QuizItems.Remove(quiz);
            return await SaveAllAsync();
        }

        public async Task<bool> DeleteQuizQuestionByIdAsync(string userId, long id) {
            var question = await GetQuizQuestionByIdAsync(userId, id);
            if (question == null) return false;
            _context.QuizQuestionItems.Remove(question);
            return await SaveAllAsync();
        }

        public async Task<bool> DeleteQuizQuestionAnswerByIdAsync(string userId, long id) {
            var answer = await GetQuizQuestionAnswerByIdAsync(userId, id);
            if (answer == null) return false;
            _context.QuizQuestionAnswerItems.Remove(answer);
            return await SaveAllAsync();
        }
    }
}
