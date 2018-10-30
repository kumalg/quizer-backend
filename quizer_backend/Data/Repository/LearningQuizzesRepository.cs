using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities.LearningQuiz;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Repository.Interfaces;
using quizer_backend.Models;

namespace quizer_backend.Data.Repository {
    public class LearningQuizzesRepository : QuizerRepository, ILearningQuizzesRepository {

        public LearningQuizzesRepository(QuizerContext context) : base(context) { }

        public async Task<LearningQuiz> GetLearningQuizByIdAsync(string userId, long id, bool includeQuiz = false) {
            var learningQuizesQuery = Context.LearningQuizItems.Where(s => s.Id == id);

            if (includeQuiz)
                learningQuizesQuery = learningQuizesQuery.Include(s => s.Quiz);

            var learningQuiz = await learningQuizesQuery.SingleOrDefaultAsync();

            if (learningQuiz?.QuizId == null)
                return null;

            var access = await UserAccessToQuizAsync(userId, learningQuiz.QuizId.Value);
            if (access == null || access.Access == QuizAccessEnum.None)
                return null;

            if (includeQuiz)
                learningQuiz.Quiz.IncludeAccess(access.Access);

            return learningQuiz;
        }

        public async Task<IQueryable<LearningQuiz>> GetAllLearningQuizes(string userId, bool includeQuiz = false) {
            var learningQuizzes = Context.LearningQuizItems.Where(q => q.UserId == userId);

            if (includeQuiz) {
                learningQuizzes = learningQuizzes.Include(q => q.Quiz);
                foreach (var learningQuiz in learningQuizzes)
                    await IncludeAccess(learningQuiz.Quiz, userId);
            }

            return learningQuizzes;
        }

        public IQueryable<LearningQuizQuestionReoccurrences> GetLearningQuizQuestionsReoccurrences(long learningQuizId) {
            return Context.LearningQuizQuestionReoccurrencesItems
                .Where(q => q.LearningQuizId == learningQuizId);
        }

        public async Task<LearningQuizQuestionReoccurrences> GetLearningQuizQuestionReoccurrences(long learningQuizId, long questionId) {
            return await Context.LearningQuizQuestionReoccurrencesItems
                .Where(q => q.LearningQuizId == learningQuizId)
                .Where(q => q.QuizQuestionId == questionId)
                .SingleOrDefaultAsync();
        }

        public async Task<bool?> IsLearningQuizFinished(string userId, long learningQuizId) {
            var learningQuiz = await Context.LearningQuizItems
                .Where(l => l.UserId == userId)
                .Where(l => l.Id == learningQuizId)
                .Include(i => i.Reoccurrences)
                .SingleOrDefaultAsync();

            var isAnyRemaining = learningQuiz?.Reoccurrences.Any(r => r.Reoccurrences > 0);
            return !isAnyRemaining;
        }

        public async Task<bool> FinishLearningQuiz(string userId, long learningQuizId) {
            var learningQuiz = await Context.LearningQuizItems
                .Where(l => l.UserId == userId)
                .Where(l => l.Id == learningQuizId)
                .SingleOrDefaultAsync();

            learningQuiz.IsFinished = true;
            return await SaveAllAsync();
        }

        public async Task<bool> AddLearningQuizAsync(LearningQuiz learningQuiz) {
            if (learningQuiz.QuizId == null)
                return false;

            var access = await UserAccessToQuizAsync(learningQuiz.UserId, learningQuiz.QuizId.Value);
            if (access == null || access.Access == QuizAccessEnum.None)
                return false;

            learningQuiz.CreationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var questions = await GetQuizQuestionsByQuizIdAsync(learningQuiz.UserId, learningQuiz.QuizId.Value, learningQuiz.CreationTime);

            learningQuiz.NumberOfQuestions = questions.Count();
            await Context.LearningQuizItems.AddAsync(learningQuiz);

            var reoccurrences = questions.Select(q => new LearningQuizQuestionReoccurrences {
                LearningQuizId = learningQuiz.Id,
                QuizQuestionId = q.Id,
                Reoccurrences = 2
            });

            await Context.LearningQuizQuestionReoccurrencesItems.AddRangeAsync(reoccurrences);
            return await SaveAllAsync();
        }
    }
}
