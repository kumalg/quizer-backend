using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities.LearningQuiz;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Entities.SolvedQuiz;
using quizer_backend.Data.Repository;
using quizer_backend.Models;

namespace quizer_backend.Data.Services {
    public class StatisticsService : BaseService {

        private readonly QuizzesRepository _quizzesRepository;
        private readonly QuizAccessesRepository _quizAccessesRepository;
        private readonly SolvedQuizRepository _solvedQuizRepository;
        private readonly LearningQuizzesRepository _learningQuizzesRepository;
        private readonly QuizSessionsRepository _quizSessionsRepository;

        public StatisticsService(QuizerContext context) : base(context) {
            _quizzesRepository = new QuizzesRepository(context);
            _quizAccessesRepository = new QuizAccessesRepository(context);
            _solvedQuizRepository = new SolvedQuizRepository(context);
            _learningQuizzesRepository = new LearningQuizzesRepository(context);
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
                .ThenInclude(q => q.Versions.OrderByDescending(v => v.CreationTime).FirstOrDefault())
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
                .ThenInclude(q => q.Versions)
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
                .ThenInclude(q => q.Versions)
                .Include(s => s.Questions)
                .ThenInclude(q => q.Answers)
                .ThenInclude(q => q.Answer)
                .ToListAsync();
        }

        public async Task<List<LearningQuiz>> GetLearningQuizzesByQuizId(Guid quizId) {
            return await _learningQuizzesRepository
                .GetAll()
                .Where(l => l.QuizId == quizId)
                .Include(l => l.Quiz)
                .Include(q => q.LearningQuizQuestions)
                .ThenInclude(lq => lq.Question)
                .ThenInclude(q => q.Versions)

                //.ThenInclude(q => q.Question)
                //.ThenInclude(q => q.Versions)
                //.Include(s => s.Questions)
                //.ThenInclude(q => q.Answers)
                //.ThenInclude(q => q.Answer)
                .ToListAsync();
        }

        public async Task<List<LearningQuiz>> GetLearningQuizzesByUserId(string userId) {
            return await _learningQuizzesRepository
                .GetAll()
                .Where(l => l.UserId == userId)
                .Include(l => l.Quiz)
                .Include(q => q.LearningQuizQuestions)
                .ThenInclude(lq => lq.Question)
                .ThenInclude(q => q.Versions)

                //.ThenInclude(q => q.Question)
                //.ThenInclude(q => q.Versions)
                //.Include(s => s.Questions)
                //.ThenInclude(q => q.Answers)
                //.ThenInclude(q => q.Answer)
                .ToListAsync();
        }

        public async Task<bool> IncreaseLearnSessions(Guid quizId) {
            var quiz = await _quizSessionsRepository.GetById(quizId);
            if (quiz == null)
                return false;
            
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
            if (access == null || access.Access != QuizAccessEnum.Owner && access.Access != QuizAccessEnum.Creator)
                return null;
            return await GetQuizStatistics(quizId);
        }

        private async Task<object> GetLearningStatistics(Guid quizId) {
            var learningQuizzes = await GetLearningQuizzesByQuizId(quizId);
            var startedCount = learningQuizzes.Count;
            var finishedCount = learningQuizzes.Count(l => l.IsFinished);

            var correctSum = learningQuizzes.Sum(s => s.NumberOfCorrectAnswers);
            var badSum = learningQuizzes.Sum(s => s.NumberOfBadAnswers);

            var usersCount = learningQuizzes.Select(s => s.UserId).Distinct().Count();
            
            var questionsDifficulty = learningQuizzes
                .SelectMany(s => s.LearningQuizQuestions)
                .GroupBy(q => q.QuestionId)
                .Select(g => new {
                    CorrectCount = g.Sum(q => q.GoodUserAnswers),
                    BadCount = g.Sum(q => q.BadUserAnswers),
                    Question = g.First().Question.FlatVersionProps()
                })
                .Select(g => new {
                    CorrectCount = g.CorrectCount,
                    BadCount = g.BadCount,
                    Question = g.Question,
                    Points = g.CorrectCount - g.BadCount
                })
                //.Where(q => !(q.BadCount == 0 && q.CorrectCount == 0) )
                .OrderByDescending(g => g.Points)
                .AsQueryable();

            var difficultQuestions = questionsDifficulty
                .TakeLast(3)
                .OrderBy(a => a.Points);

            var easyQuestions = questionsDifficulty
                .Take(3);

            return new {
                UsersCount = usersCount,
                StartedCount = startedCount,
                FinishedCount = finishedCount,
                CorrectSum = correctSum,
                BadSum = badSum,
                MostDifficultQuestions = difficultQuestions,
                MostEasyQuestions = easyQuestions
            };
        }

        private async Task<object> GetSolvingStatistics(Guid quizId) {
            var solvingQuizzes = await GetSolvedQuizzesByQuizId(quizId);
            var correctSum = solvingQuizzes.Sum(s => s.CorrectCount);
            var badSum = solvingQuizzes.Sum(s => s.BadCount);
            var usersCount = solvingQuizzes.Select(s => s.UserId).Distinct().Count();
            var sum = correctSum + badSum;
            var averageScore = sum == 0
                ? 0
                : (double)correctSum / (correctSum + badSum);
            var averageSolvingTime = solvingQuizzes.Any()
                ? solvingQuizzes.Sum(s => s.SolveTime) / solvingQuizzes.Count
                : 0;

            var questionsDifficulty = solvingQuizzes
                .SelectMany(s => s.Questions)
                .GroupBy(q => q.QuestionId)
                .Select(g => new {
                    CorrectCount = g.Count(q => q.AnsweredCorrectly),
                    BadCount = g.Count(q => !q.AnsweredCorrectly),
                    Question = g.First().Question.FlatVersionProps()
                })
                .Select(g => new {
                    CorrectCount = g.CorrectCount,
                    BadCount = g.BadCount,
                    Question = g.Question,
                    Points = g.CorrectCount - g.BadCount
                })
                //.Where(q => !(q.BadCount == 0 && q.CorrectCount == 0))
                .OrderByDescending(g => g.Points)
                .AsQueryable();

            var difficultQuestions = questionsDifficulty
                .TakeLast(3)
                .OrderBy(a => a.Points);

            var easyQuestions = questionsDifficulty
                .Take(3);

            return new {
                UsersCount = usersCount,
                AverageScore = averageScore,
                AverageTime = averageSolvingTime,
                MostDifficultQuestions = difficultQuestions,
                MostEasyQuestions = easyQuestions
            };
        }

        private async Task<object> GetQuizStatistics(Guid quizId) {
            var quiz = await _quizzesRepository
                .GetAll()
                .Where(q => q.Id == quizId)
                .Include(q => q.Sessions)
                .SingleOrDefaultAsync();

            return new {
                Quiz = quiz,
                NumberOfLearnSessions = quiz.Sessions.NumberOfLearnSessions,
                NumberOfSolveSessions = quiz.Sessions.NumberOfSolveSessions,
                SolvingStatistics = await GetSolvingStatistics(quizId),
                LearningStatistics = await GetLearningStatistics(quizId)
            };
        }

        public async Task<object> GetUserStatistics(string userId) {
            var solved = await GetSolvedQuizzesByUserId(userId);
            var solvedCount = solved.Count;
            var solvedDistinctCount = solved.Select(s => s.QuizId).Distinct().Count();
            var mostPopularSolved = solved
                .GroupBy(s => s.Quiz)
                .Select(s => new {
                    Quiz = s.Key,
                    Count = s.Count()
                })
                .OrderByDescending(s => s.Count)
                .Take(3);

            var learning = await GetLearningQuizzesByUserId(userId);
            var learningCount = learning.Count;
            var learningDistinctCount = learning.Select(s => s.QuizId).Distinct().Count();
            var mostPopularLearning = learning
                .GroupBy(s => s.Quiz)
                .Select(s => new {
                    Quiz = s.Key,
                    Count = s.Count()
                })
                .OrderByDescending(s => s.Count)
                .Take(3);

            return new {
                SolvedCount = solvedCount,
                SolvedDistinctCount = solvedDistinctCount,
                MostPopularSolved = mostPopularSolved,
                LearningCount = learningCount,
                LearningDistinctCount = learningDistinctCount,
                MostPopularLearning = mostPopularLearning
            };
        }

        public async Task<List<Quiz>> GetQuizListForStatistics(string userId) {
            return await _quizAccessesRepository
                .GetAll()
                .Where(a => a.UserId == userId)
                .Where(a => a.Access == QuizAccessEnum.Owner || a.Access == QuizAccessEnum.Creator)
                .Include(a => a.Quiz)
                .Select(a => a.Quiz)
                .ToListAsync();
        }
    }
}
