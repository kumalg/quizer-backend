using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities.SolvedQuiz;
using quizer_backend.Data.Repository;
using quizer_backend.Models;

namespace quizer_backend.Data.Services {
    public class StatisticsService : BaseService {

        private readonly QuizzesRepository _quizzesRepository;
        private readonly QuizAccessesRepository _quizAccessesRepository;
        private readonly SolvedQuizRepository _solvedQuizRepository;
        private readonly QuizSessionsRepository _quizSessionsRepository;

        public StatisticsService(QuizerContext context) : base(context) {
            _quizzesRepository = new QuizzesRepository(context);
            _quizAccessesRepository = new QuizAccessesRepository(context);
            _solvedQuizRepository = new SolvedQuizRepository(context);
            _quizSessionsRepository = new QuizSessionsRepository(context);
        }


        // Create/Update/Delete methods

        public async Task<SolvedQuiz> CreateSolvedQuiz(UserSolvedQuiz solvedQuiz, int correctCount, int badCount, long finishTime, List<SolvedQuestion> questions, string userId) {
            var statsQuiz = new SolvedQuiz {
                QuizId = solvedQuiz.QuizId,
                UserId = userId,
                CreationTime = solvedQuiz.CreatedTime,
                FinishTime = finishTime,
                SolveTime = solvedQuiz.SolvingTime,
                CorrectCount = correctCount,
                BadCount = badCount,
                Questions = questions
            };
            await _solvedQuizRepository.Create(statsQuiz);
            var result = await Context.SaveChangesAsync() > 0;
            return result
                ? statsQuiz
                : null;
        }

        public async Task<SolvedQuiz> GetSolvedQuizById(long id) {
            return await _solvedQuizRepository
                .GetAll()
                .Where(s => s.Id == id)
                .Include(s => s.Quiz)
                .Include(s => s.Questions)
                .ThenInclude(q => q.Question)
                .Include(s => s.Questions)
                .ThenInclude(q => q.Answers)
                .ThenInclude(q => q.Answer)
                .SingleOrDefaultAsync();
        }

        public async Task<List<SolvedQuiz>> GetSolvedQuizzesByQuizId(Guid quizId) {
            return await _solvedQuizRepository
                .GetAll()
                .Where(s => s.QuizId == quizId)
                .Include(s => s.Quiz)
                .Include(s => s.Questions)
                .ThenInclude(q => q.Question)
                .Include(s => s.Questions)
                .ThenInclude(q => q.Answers)
                .ThenInclude(q => q.Answer)
                .ToListAsync();
        }

        public async Task<List<SolvedQuiz>> GetSolvedQuizzesByUserId(string userId) {
            return await _solvedQuizRepository
                .GetAll()
                .Where(s => s.UserId == userId)
                .Include(s => s.Quiz)
                .Include(s => s.Questions)
                .ThenInclude(q => q.Question)
                .Include(s => s.Questions)
                .ThenInclude(q => q.Answers)
                .ThenInclude(q => q.Answer)
                .ToListAsync();
        }

        public async Task<bool> IncreaseLearnSessions(Guid quizId) {
            var quiz = await _quizSessionsRepository.GetById(quizId);
            if (quiz == null)
                return false;

            //using (var context = serviceProvider.GetService<QuizerContext>()) {
            bool saveFailed;
            do {
                saveFailed = false;

                ++quiz.NumberOfLearnSessions;

                try {
                    await Context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e) {
                    saveFailed = true;
                    e.Entries.Single().Reload();
                }
            } while (saveFailed);
            //}

            return true;
        }

        public async Task<bool> IncreaseSolveSessions(Guid quizId) {
            var quiz = await _quizSessionsRepository.GetById(quizId);
            if (quiz == null)
                return false;
            
            bool saveFailed;
            do {
                saveFailed = false;

                ++quiz.NumberOfSolveSessions;

                try {
                    await Context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e) {
                    saveFailed = true;
                    e.Entries.Single().Reload();
                }
            } while (saveFailed);

            return true;
        }

        public async Task<object> GetQuizStatistics(Guid quizId, string userId) {
            var access = await _quizAccessesRepository.GetQuizAccessForUserAsync(userId, quizId);
            if (access == null || access.Access != QuizAccessEnum.Owner || access.Access != QuizAccessEnum.Creator)
                return null;
            return await GetQuizStatistics(quizId);
        }

        private async Task<object> GetQuizStatistics(Guid quizId) {
            var quiz = await _quizzesRepository
                .GetAll()
                .Where(q => q.Id == quizId)
                .Include(q => q.Sessions)
                .SingleOrDefaultAsync();
            var solvingQuizzes = await GetSolvedQuizzesByQuizId(quizId);
            var correctSum = solvingQuizzes.Sum(s => s.CorrectCount);
            var badSum = solvingQuizzes.Sum(s => s.BadCount);
            var sum = correctSum + badSum;
            var averageScore = sum == 0
                ? 0
                : (double) correctSum / (correctSum + badSum);
            var averageSolvingTime = solvingQuizzes.Any()
                ? solvingQuizzes.Sum(s => s.SolveTime) / solvingQuizzes.Count
                : 0;

            var questionsDifficulty = solvingQuizzes
                .SelectMany(s => s.Questions)
                .GroupBy(q => q.QuestionId)
                .AsQueryable()
                .Select(g => new {
                    CorrectCount = g.Count(q => q.AnsweredCorrectly),
                    BadCount = g.Count(q => !q.AnsweredCorrectly),
                    Question = g.First().Question
                })
                .Select(g => new {
                    CorrectCount = g.CorrectCount,
                    BadCount = g.BadCount,
                    Question = g.Question,
                    Points = g.CorrectCount - g.BadCount
                })
                .OrderByDescending(g => g.Points);

            var difficultQuestions = questionsDifficulty.TakeLast(5).OrderBy(a => a.Points);
            var easyQuestions = questionsDifficulty.Take(5);

            return new {
                Quiz = quiz,
                NumberOfLearnSessions = quiz.Sessions.NumberOfLearnSessions,
                NumberOfSolveSessions = quiz.Sessions.NumberOfSolveSessions,
                SolveAverageScore = averageScore,
                AverageSolvingTime = averageSolvingTime,
                MostDifficultQuestions = difficultQuestions,
                MostEasyQuestions = easyQuestions
            };
        }
    }
}
