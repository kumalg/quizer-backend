using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities;
using quizer_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Entities.QuizObjectVersion;

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
                                       i.UserId == userId
                                   )
                                  .FirstOrDefaultAsync();
        }


        // GETOS

        public async Task<SolvingQuiz> GetSolvingQuizByIdAsync(string userId, long id) {
            var solvingQuiz = await _context.SolvingQuizItems
                                 .Where(s => s.Id == id)
                                 .Include(s => s.Quiz)
                                 .FirstOrDefaultAsync();

            var access = await UserAccessToQuizAsync(userId, solvingQuiz.QuizId ?? -1);
            if (access == null || access.Access == QuizAccessEnum.None)
                return null;

            solvingQuiz.Quiz.IncludeAccess(access.Access);
            return solvingQuiz;
        }

        public async Task<Quiz> GetQuizByIdAsync(string userId, long id) {
            var access = await UserAccessToQuizAsync(userId, id);

            if (access == null || access.Access == QuizAccessEnum.None)
                return null;

            var quiz = await _context.QuizItems.FirstOrDefaultAsync(i => i.Id == id);
            return quiz.IncludeAccess(access.Access);
        }

        public async Task<QuizQuestion> GetQuizQuestionByIdAsync(string userId, long id, long? maxTime = null, long? minTime = null) {
            var question = await _context.QuizQuestionItems
                                         .Where(i => i.Id == id)
                                         .Include(i => i.Versions)
                                         .FirstOrDefaultAsync();

            if (question == null || !await _context.QuizAccessItems.AnyAsync(i => i.QuizId == question.QuizId && i.UserId == userId))
                return null;

            return question;
        }
        
        public async Task<QuizQuestionAnswer> GetQuizQuestionAnswerByIdAsync(string userId, long id, long? maxTime = null, long? minTime = null) {
            var answer = await _context.QuizQuestionAnswerItems
                                       .Where(i => i.Id == id)
                                       .Include(i => i.QuizQuestion)
                                       .Include(i => i.Versions)
                                       .FirstOrDefaultAsync();

            if (answer == null || !await _context.QuizAccessItems.AnyAsync(i => i.QuizId == answer.QuizQuestion.QuizId && i.UserId == userId))
                return null;

            return answer;
        }

        public async Task<IQueryable<SolvingQuiz>> GetAllSolvingQuizes(string userId) {
            var solvingQuizes = _context.SolvingQuizItems
                                        .Where(q => q.UserId == userId)
                                        .Include(q => q.Quiz);

            foreach (var solvingQuiz in solvingQuizes) {
                var access = await UserAccessToQuizAsync(userId, solvingQuiz.Quiz.Id);
                solvingQuiz.Quiz.Access = access.Access;
            }

            return solvingQuizes;
        }

        public IQueryable<Quiz> GetAllQuizes(string userId) {
            return _context.QuizAccessItems
                           .Where(i => i.UserId == userId && i.Access != QuizAccessEnum.None)
                           .Include(b => b.Quiz)
                           .Select(p => p.Quiz.IncludeAccess(p.Access));
        }

        public async Task<IQueryable<QuizQuestion>> GetQuizQuestionsByQuizIdAsync(string userId, long id, long? maxTime, long? minTime) {
            var access = await UserAccessToQuizAsync(userId, id);

            if (access == null || access.Access == QuizAccessEnum.None)
                return null;

            var questions = _context.QuizQuestionItems.Where(i => i.QuizId == id);

            if (maxTime != null) questions = questions.Where(i => i.CreationTime <= maxTime);
            if (minTime != null) questions = questions.Where(i => i.CreationTime >= minTime);

            return questions.Include(q => q.Versions);
        }

        public async Task<IQueryable<QuizQuestionAnswer>> GetQuizQuestionAnswersByQuizQuestionIdAsync(string userId, long id, long? maxTime, long? minTime) {
            var question = await _context.QuizQuestionItems
                                         .FirstOrDefaultAsync(i => i.Id == id);

            if (question == null)
                return null;

            var access = await UserAccessToQuizAsync(userId, question.QuizId);
            if (access == null || access.Access == QuizAccessEnum.None)
                return null;

            return _context.QuizQuestionAnswerItems
                           .Where(a => a.QuizQuestionId == question.Id)
                           .Include(a => a.Versions);
        }


        // ADDOS

        public async Task<bool> AddQuizAsync(Quiz quiz) {
            long creationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            quiz.CreationTime = creationTime;
            quiz.LastModifiedTime = creationTime;

            await _context.QuizItems.AddAsync(quiz);
            return await SaveAllAsync();
        }

        public async Task<bool> AddQuizQuestionWithVersionAsync(QuizQuestion question) {
            long creationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            question.CreationTime = creationTime;

            await _context.QuizQuestionItems.AddAsync(question);

            var questionVersion = new QuizQuestionVersion {
                CreationTime = creationTime,
                QuizQuestionId = question.Id,
                Value = question.Value
            };
            await _context.QuizQuestionVersionItems.AddAsync(questionVersion);

            return await SaveAllAsync();
        }

        public async Task<bool> AddQuizQuestionAnswerWithVersionAsync(QuizQuestionAnswer answer) {
            long creationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            answer.CreationTime = creationTime;

            await _context.QuizQuestionAnswerItems.AddAsync(answer);

            var answerVersion = new QuizQuestionAnswerVersion {
                CreationTime = creationTime,
                QuizQuestionAnswerId = answer.Id,
                Value = answer.Value,
                IsCorrect = answer.IsCorrect
            };
            await _context.QuizQuestionAnswerVersionItems.AddAsync(answerVersion);

            return await SaveAllAsync();
        }

        public async Task<bool> AddQuizAccessAsync(QuizAccess access) {
            await _context.QuizAccessItems.AddAsync(access);
            return await SaveAllAsync();
        }

        public async Task<bool> AddQuizQuestionVersionAsync(QuizQuestionVersion questionVersion) {
            questionVersion.CreationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            await _context.QuizQuestionVersionItems.AddAsync(questionVersion);
            return await SaveAllAsync();
        }

        public async Task<bool> AddQuizQuestionAnswerVersionAsync(QuizQuestionAnswerVersion answerVersion) {
            answerVersion.CreationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            await _context.QuizQuestionAnswerVersionItems.AddAsync(answerVersion);
            return await SaveAllAsync();
        }

        public async Task<bool> AddSolvingQuizAsync(SolvingQuiz solvingQuiz) {
            solvingQuiz.CreationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            await _context.SolvingQuizItems.AddAsync(solvingQuiz);
            return await SaveAllAsync();
        }


        // DELETOS

        public async Task<bool> DeleteQuizByIdAsync(string userId, long id) {
            var access = await UserAccessToQuizAsync(userId, id);
            if (access == null || access.Access == QuizAccessEnum.None)
                return false;

            _context.QuizItems.Remove(access.Quiz);
            return await SaveAllAsync();
        }

        public async Task<bool> DeleteQuizQuestionByIdAsync(string userId, long id) {
            var question = await GetQuizQuestionByIdAsync(userId, id);
            if (question == null)
                return false;

            var access = await UserAccessToQuizAsync(userId, question.QuizId);
            if (access == null || access.Access == QuizAccessEnum.None)
                return false;

            _context.QuizQuestionItems.Remove(question);
            return await SaveAllAsync();
        }

        public async Task<bool> DeleteQuizQuestionAnswerByIdAsync(string userId, long id) {
            var answer = await GetQuizQuestionAnswerByIdAsync(userId, id);
            if (answer == null)
                return false;

            var access = await UserAccessToQuizAsync(userId, answer.QuizQuestion.QuizId);
            if (access == null || access.Access == QuizAccessEnum.None)
                return false;

            _context.QuizQuestionAnswerItems.Remove(answer);
            return await SaveAllAsync();
        }

        public async Task<bool> DeleteSolvingQuizAsync(string userId, long id) {
            var solvingQuiz = await _context.SolvingQuizItems
                                            .FirstOrDefaultAsync(s =>
                                                s.Id == id &&
                                                s.UserId == userId
                                            );

            if (solvingQuiz == null)
                return false;

            _context.SolvingQuizItems.Remove(solvingQuiz);
            return await SaveAllAsync();
        }

        public async Task<List<SolvingQuizFinishedQuestion>> GetSolvingQuizFinishedQuestions(string userId, long id, long? maxTime, long? minTime) {
            var solvingQuiz = await _context.SolvingQuizItems
                                            .Where(s =>
                                                s.Id == id &&
                                                s.UserId == userId)
                                            .Include(s => s.FinishedQuestions)
                                            .FirstOrDefaultAsync();
            return solvingQuiz?.FinishedQuestions;
        }
    }
}
