using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Entities.QuizObjectVersion;
using quizer_backend.Data.Entities.SolvingQuiz;
using quizer_backend.Data.Repository.Interfaces;
using quizer_backend.Models;

namespace quizer_backend.Data.Repository {
    public class QuizerRepository : QuizerRepositoryBase, IQuizerRepository {

        public QuizerRepository(QuizerContext context) : base(context) { }


        // GETOS

        public async Task<SolvingQuiz> GetSolvingQuizByIdAsync(string userId, long id) {
            var solvingQuiz = await Context.SolvingQuizItems
                .Where(s => s.Id == id)
                .Include(s => s.Quiz)
                .SingleOrDefaultAsync();

            if (solvingQuiz.QuizId == null)
                return null;

            var access = await UserAccessToQuizAsync(userId, solvingQuiz.QuizId.Value);
            if (access == null || access.Access == QuizAccessEnum.None)
                return null;

            solvingQuiz.Quiz.IncludeAccess(access.Access);
            return solvingQuiz;
        }
        

        public async Task<Quiz> GetQuizByIdAsync(string userId, long id) {
            var access = await UserAccessToQuizAsync(userId, id);
            if (access == null || access.Access == QuizAccessEnum.None)
                return null;

            var quiz = await Context.QuizItems
                .Where(i => i.Id == id)
                .SingleOrDefaultAsync();

            return quiz.IncludeAccess(access.Access);
        }

        public async Task<QuizQuestion> GetQuizQuestionByIdAsync(string userId, long questionId, long? maxTime = null, long? minTime = null) {
            var question = await Context.QuizQuestionItems
                .Where(i => i.Id == questionId)
                .Include(i => i.Versions)
                .SingleOrDefaultAsync();

            if (question == null) return null;
            if (maxTime != null && question.CreationTime > maxTime) return null;
            if (minTime != null && question.CreationTime < minTime) return null;
            
            var access = await UserAccessToQuizAsync(userId, question.QuizId);
            if (access == null || access.Access == QuizAccessEnum.None)
                return null;

            return question;
        }

        public async Task<QuizQuestionAnswer> GetQuizQuestionAnswerByIdAsync(string userId, long id, long? maxTime = null, long? minTime = null) {
            var answer = await Context.QuizQuestionAnswerItems
                .Where(i => i.Id == id)
                .Include(i => i.QuizQuestion)
                .Include(i => i.Versions)
                .SingleOrDefaultAsync();

            if (answer == null)
                return null;

            var access = await UserAccessToQuizAsync(userId, id);
            if (access == null || access.Access != QuizAccessEnum.None)
                return null;

            return answer;
        }

        public async Task<IQueryable<SolvingQuiz>> GetAllSolvingQuizes(string userId) {
            var solvingQuizes = Context.SolvingQuizItems
                .Where(q => q.UserId == userId)
                .Include(q => q.Quiz);

            foreach (var solvingQuiz in solvingQuizes)
                await IncludeAccess(solvingQuiz.Quiz, userId);
            
            return solvingQuizes;
        }

        public IQueryable<Quiz> GetAllQuizes(string userId) {
            return Context.QuizAccessItems
                .Where(i => i.UserId == userId)
                .Where(i => i.Access != QuizAccessEnum.None)
                .Include(b => b.Quiz)
                .Select(p => p.Quiz.IncludeAccess(p.Access));
        }

        public async Task<IQueryable<QuizQuestion>> GetQuizQuestionsByQuizIdAsync(string userId, long id, long? maxTime = null, long? minTime = null) {
            var access = await UserAccessToQuizAsync(userId, id);
            if (access == null || access.Access == QuizAccessEnum.None)
                return null;

            var questions = Context.QuizQuestionItems.Where(i => i.QuizId == id);

            if (maxTime != null) questions = questions.Where(i => i.CreationTime <= maxTime);
            if (minTime != null) questions = questions.Where(i => i.CreationTime >= minTime);

            return questions.Include(q => q.Versions);
        }

        public async Task<IQueryable<QuizQuestionAnswer>> GetQuizQuestionAnswersByQuizQuestionIdAsync(string userId, long id, long? maxTime, long? minTime) {
            var question = await Context.QuizQuestionItems
                .Where(i => i.Id == id)
                .SingleOrDefaultAsync();

            if (question == null)
                return null;

            var access = await UserAccessToQuizAsync(userId, question.QuizId);
            if (access == null || access.Access == QuizAccessEnum.None)
                return null;

            return Context.QuizQuestionAnswerItems
                .Where(a => a.QuizQuestionId == question.Id)
                .Include(a => a.Versions);
        }
        

        // ADDOS

        public async Task<bool> AddQuizAsync(Quiz quiz) {
            long creationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            quiz.CreationTime = creationTime;
            quiz.LastModifiedTime = creationTime;

            await Context.QuizItems.AddAsync(quiz);
            return await SaveAllAsync();
        }

        public async Task<bool> AddQuizQuestionWithVersionAsync(QuizQuestion question) {
            long creationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            question.CreationTime = creationTime;

            await Context.QuizQuestionItems.AddAsync(question);

            var questionVersion = new QuizQuestionVersion {
                CreationTime = creationTime,
                QuizQuestionId = question.Id,
                Value = question.Value
            };
            await Context.QuizQuestionVersionItems.AddAsync(questionVersion);

            return await SaveAllAsync();
        }

        public async Task<bool> AddQuizQuestionAnswerWithVersionAsync(QuizQuestionAnswer answer) {
            long creationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            answer.CreationTime = creationTime;

            await Context.QuizQuestionAnswerItems.AddAsync(answer);

            var answerVersion = new QuizQuestionAnswerVersion {
                CreationTime = creationTime,
                QuizQuestionAnswerId = answer.Id,
                Value = answer.Value,
                IsCorrect = answer.IsCorrect
            };
            await Context.QuizQuestionAnswerVersionItems.AddAsync(answerVersion);

            return await SaveAllAsync();
        }

        public async Task<bool> AddQuizAccessAsync(QuizAccess access) {
            await Context.QuizAccessItems.AddAsync(access);
            return await SaveAllAsync();
        }

        public async Task<bool> AddQuizQuestionVersionAsync(QuizQuestionVersion questionVersion) {
            questionVersion.CreationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            await Context.QuizQuestionVersionItems.AddAsync(questionVersion);
            return await SaveAllAsync();
        }

        public async Task<bool> AddQuizQuestionAnswerVersionAsync(QuizQuestionAnswerVersion answerVersion) {
            answerVersion.CreationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            await Context.QuizQuestionAnswerVersionItems.AddAsync(answerVersion);
            return await SaveAllAsync();
        }

        public async Task<bool> AddSolvingQuizAsync(SolvingQuiz solvingQuiz) {
            solvingQuiz.CreationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            await Context.SolvingQuizItems.AddAsync(solvingQuiz);
            return await SaveAllAsync();
        }
        
        
        // DELETOS

        public async Task<bool> DeleteQuizByIdAsync(string userId, long id) {
            var access = await UserAccessToQuizAsync(userId, id);
            if (access == null || access.Access == QuizAccessEnum.None)
                return false;

            Context.QuizItems.Remove(access.Quiz);
            return await SaveAllAsync();
        }

        public async Task<bool> DeleteQuizQuestionByIdAsync(string userId, long id) {
            var question = await GetQuizQuestionByIdAsync(userId, id);
            if (question == null)
                return false;

            var access = await UserAccessToQuizAsync(userId, question.QuizId);
            if (access == null || access.Access == QuizAccessEnum.None)
                return false;

            Context.QuizQuestionItems.Remove(question);
            return await SaveAllAsync();
        }

        public async Task<bool> DeleteQuizQuestionAnswerByIdAsync(string userId, long id) {
            var answer = await GetQuizQuestionAnswerByIdAsync(userId, id);
            if (answer == null)
                return false;

            var access = await UserAccessToQuizAsync(userId, answer.QuizQuestion.QuizId);
            if (access == null || access.Access == QuizAccessEnum.None)
                return false;

            Context.QuizQuestionAnswerItems.Remove(answer);
            return await SaveAllAsync();
        }

        public async Task<bool> DeleteSolvingQuizAsync(string userId, long id) {
            var solvingQuiz = await GetSolvingQuizByIdAsync(userId, id);

            if (solvingQuiz == null)
                return false;

            Context.SolvingQuizItems.Remove(solvingQuiz);
            return await SaveAllAsync();
        }

        public async Task<List<SolvingQuizFinishedQuestion>> GetSolvingQuizFinishedQuestions(string userId, long id, long? maxTime, long? minTime) {
            var solvingQuiz = await Context.SolvingQuizItems
                .Where(s => s.Id == id)
                .Where(s => s.UserId == userId)
                .Include(s => s.FinishedQuestions)
                .SingleOrDefaultAsync();

            return solvingQuiz?.FinishedQuestions;
        }
    }
}
