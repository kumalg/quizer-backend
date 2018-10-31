using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Entities.QuizObjectVersion;
using quizer_backend.Data.Repository.Interfaces;
using quizer_backend.Models;

namespace quizer_backend.Data.Repository {
    public class QuizerRepository : QuizerRepositoryBase, IQuizerRepository {

        public QuizerRepository(QuizerContext context) : base(context) { }


        // GETOS

        public async Task<Quiz> GetQuizByIdAsync(string userId, long id) {
            var access = await UserAccessToQuizAsync(userId, id);
            if (access == null || access.Access == QuizAccessEnum.None)
                return null;

            var quiz = await Context.Quizzes
                .Where(i => i.Id == id)
                .SingleOrDefaultAsync();

            return quiz.IncludeAccess(access.Access);
        }

        public async Task<Question> GetQuestionByIdAsync(string userId, long questionId, long? maxTime = null, long? minTime = null) {
            var question = await Context.Questions
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

        public async Task<Answer> GetAnswerByIdAsync(string userId, long answerId, long? maxTime = null, long? minTime = null) {
            var answer = await Context.Answers
                .Where(i => i.Id == answerId)
                .Include(i => i.Question)
                .Include(i => i.Versions)
                .SingleOrDefaultAsync();

            if (answer == null)
                return null;

            var access = await UserAccessToQuizAsync(userId, answer.Question.QuizId);
            if (access == null || access.Access == QuizAccessEnum.None)
                return null;

            return answer;
        }

        public IQueryable<Quiz> GetAllQuizzes(string userId) {
            return Context.QuizAccessItems
                .Where(i => i.UserId == userId)
                .Where(i => i.Access != QuizAccessEnum.None)
                .Include(b => b.Quiz)
                .Select(p => p.Quiz.IncludeAccess(p.Access));
        }

        public async Task<IQueryable<Question>> GetQuestionsByQuizIdAsync(string userId, long id, long? maxTime = null, long? minTime = null) {
            var access = await UserAccessToQuizAsync(userId, id);
            if (access == null || access.Access == QuizAccessEnum.None)
                return null;

            var questions = Context.Questions.Where(i => i.QuizId == id);

            if (maxTime != null) questions = questions.Where(i => i.CreationTime <= maxTime);
            if (minTime != null) questions = questions.Where(i => i.CreationTime >= minTime);

            return questions.Include(q => q.Versions);
        }

        public async Task<IQueryable<Answer>> GetAnswersByQuestionIdAsync(string userId, long id, long? maxTime, long? minTime) {
            var question = await Context.Questions
                .Where(i => i.Id == id)
                .SingleOrDefaultAsync();

            if (question == null)
                return null;

            var access = await UserAccessToQuizAsync(userId, question.QuizId);
            if (access == null || access.Access == QuizAccessEnum.None)
                return null;

            return Context.Answers
                .Where(a => a.QuestionId == question.Id)
                .Include(a => a.Versions);
        }
        

        // ADDOS

        public async Task<bool> AddQuizAsync(Quiz quiz) {
            long creationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            quiz.CreationTime = creationTime;
            quiz.LastModifiedTime = creationTime;

            await Context.Quizzes.AddAsync(quiz);
            return await SaveAllAsync();
        }

        public async Task<bool> AddQuestionWithVersionAsync(Question question) {
            long creationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            question.CreationTime = creationTime;

            await Context.Questions.AddAsync(question);

            var questionVersion = new QuestionVersion {
                CreationTime = creationTime,
                QuizQuestionId = question.Id,
                Value = question.Value
            };
            await Context.QuestionVersions.AddAsync(questionVersion);

            return await SaveAllAsync();
        }

        public async Task<bool> AddAnswerWithVersionAsync(Answer answer) {
            long creationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            answer.CreationTime = creationTime;

            await Context.Answers.AddAsync(answer);

            var answerVersion = new AnswerVersion {
                CreationTime = creationTime,
                QuizQuestionAnswerId = answer.Id,
                Value = answer.Value,
                IsCorrect = answer.IsCorrect
            };
            await Context.AnswerVersions.AddAsync(answerVersion);

            return await SaveAllAsync();
        }

        public async Task<bool> AddQuizAccessAsync(QuizAccess access) {
            await Context.QuizAccessItems.AddAsync(access);
            return await SaveAllAsync();
        }

        public async Task<bool> AddQuestionVersionAsync(QuestionVersion questionVersion) {
            questionVersion.CreationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            await Context.QuestionVersions.AddAsync(questionVersion);
            return await SaveAllAsync();
        }

        public async Task<bool> AddAnswerVersionAsync(AnswerVersion answerVersion) {
            answerVersion.CreationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            await Context.AnswerVersions.AddAsync(answerVersion);
            return await SaveAllAsync();
        }


        // DELETOS

        public async Task<bool> DeleteQuizByIdAsync(string userId, long id) {
            var access = await UserAccessToQuizAsync(userId, id);
            if (access == null || access.Access == QuizAccessEnum.None)
                return false;

            Context.Quizzes.Remove(access.Quiz);
            return await SaveAllAsync();
        }

        public async Task<bool> DeleteQuestionByIdAsync(string userId, long id) {
            var question = await GetQuestionByIdAsync(userId, id);
            if (question == null)
                return false;

            var access = await UserAccessToQuizAsync(userId, question.QuizId);
            if (access == null || access.Access == QuizAccessEnum.None)
                return false;

            Context.Questions.Remove(question);
            return await SaveAllAsync();
        }

        public async Task<bool> DeleteAnswerByIdAsync(string userId, long id) {
            var answer = await GetAnswerByIdAsync(userId, id);
            if (answer == null)
                return false;

            var access = await UserAccessToQuizAsync(userId, answer.Question.QuizId);
            if (access == null || access.Access == QuizAccessEnum.None)
                return false;

            Context.Answers.Remove(answer);
            return await SaveAllAsync();
        }
    }
}
