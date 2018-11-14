using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Repository;
using quizer_backend.Helpers;
using quizer_backend.Models;
using quizer_backend.Services;

namespace quizer_backend.Data.Services {
    public class SolvingQuizzesService : BaseService {

        private readonly QuizzesRepository _quizzesRepository;
        private readonly QuestionsRepository _questionsRepository;
        private readonly Auth0UsersService _auth0UsersService;

        public SolvingQuizzesService(
            QuizerContext context,
            Auth0ManagementFactory auth0ManagementFactory
        ) : base(context) {
            _quizzesRepository = new QuizzesRepository(context);
            _questionsRepository = new QuestionsRepository(context);
            _auth0UsersService = new Auth0UsersService(auth0ManagementFactory);
        }
        
        private IQueryable<Question> RandomQuizQuestionsQuery(Guid id) {
            return _questionsRepository
                .GetAllByQuizId(id)
                .Shuffle();
        }

        private async Task<List<SolvingQuizQuestion>> RandomQuizQuestions(IQueryable<Question> questionsQuery, int? range) {
            var random = new Random();
            var query = questionsQuery;

            if (range != null) {
                query = query.Take(range.Value);
            }

            return await query
                .Include(q => q.Versions)
                .Include(q => q.Answers)
                .ThenInclude(a => a.Versions)
                .Select(q => new SolvingQuizQuestion {
                    Id = q.Id,
                    Value = q.Versions
                        .OrderByDescending(v => v.CreationTime)
                        .FirstOrDefault()
                        .Value,
                    Answers = q.Answers
                        .Where(a => !a.IsDeleted)
                        .OrderBy(g => random.Next())
                        .Select(a => new SolvingQuizAnswer {
                            Id = a.Id,
                            Value = a.Versions
                                .OrderByDescending(v => v.CreationTime)
                                .FirstOrDefault()
                                .Value
                        })
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task<object> GetRandomQuestionsWithQuizByQuizIdAsync(Guid id, string userId) {
            var isPublic = await _quizzesRepository.IsPublicAsync(id);

            if (!isPublic) {
                var haveReadAccess = await _quizzesRepository.HaveReadAccessToQuizAsync(userId, id);
                if (!haveReadAccess)
                    return null;
            }

            var quiz = await _quizzesRepository.GetById(id);
            await _auth0UsersService.QuizItemWithOwnerNickName(quiz);

            var randomQuestionsQuery = RandomQuizQuestionsQuery(id);
            var randomQuestions = await RandomQuizQuestions(randomQuestionsQuery, quiz.QuestionsInSolvingQuiz);

            var questionsInDatabase = await randomQuestionsQuery.CountAsync();
            
            var creationTime = CurrentTime;

            return new {
                SolvingQuiz = new {
                    Quiz = quiz,
                    QuestionsInDatabase = questionsInDatabase,
                    QuestionsInSolvingQuiz = randomQuestions.Count,
                    Questions = randomQuestions,
                    CreatedTime = creationTime
                },
                SolvedQuizTemplate = new UserSolvedQuiz {
                    QuizId = quiz.Id,
                    CreatedTime = creationTime,
                    SolvingTime = 0,
                    MaxSolvingTime = quiz.MillisecondsInSolvingQuiz,
                    UserSolvedQuestions = randomQuestions
                        .Select(q => new UserSolvedQuizQuestion { QuestionId = q.Id })
                        .ToList()
                }
            };
        }

        public async Task<object> CheckSolvedQuizAsync(UserSolvedQuiz solvedQuiz, string userId) {
            var isPublic = await _quizzesRepository.IsPublicAsync(solvedQuiz.QuizId);
            if (!isPublic) {
                var haveReadAccess = await _quizzesRepository.HaveReadAccessToQuizAsync(userId, solvedQuiz.QuizId);
                if (!haveReadAccess)
                    return null;
            }

            var solvedQuestionIds = solvedQuiz.UserSolvedQuestions.Select(q => q.QuestionId);
            var questionsWithCorrectAnswers = await _questionsRepository
                .GetAllByQuizId(solvedQuiz.QuizId, solvedQuiz.CreatedTime, true)
                .Include(q => q.Versions)
                .Include(q => q.Answers)
                .ThenInclude(a => a.Versions)
                .Where(q => solvedQuestionIds.Contains(q.Id))
                .Select(q => new {
                    q.Id,
                    q.Versions
                        .Where(v => v.CreationTime <= solvedQuiz.CreatedTime)
                        .OrderByDescending(v => v.CreationTime)
                        .FirstOrDefault()
                        .Value,
                    Answers = q.Answers
                        .Where(a => !a.IsDeleted)
                        .Select(a => new {
                            a.Id,
                            Version = a.Versions
                                .Where(v => v.CreationTime <= solvedQuiz.CreatedTime)
                                .OrderByDescending(v => v.CreationTime)
                                .FirstOrDefault()
                        })
                        .Select(a => new {
                            a.Id,
                            a.Version.Value,
                            a.Version.IsCorrect
                        })
                })
                .ToListAsync();

            if (questionsWithCorrectAnswers == null || questionsWithCorrectAnswers.Count == 0)
                return null;

            var questionsWithCorrectAnswersAndUserAnswers = questionsWithCorrectAnswers
                .Select(q => new {
                    q.Id,
                    q.Value,
                    q.Answers,
                    UserSelectedAnswers = solvedQuiz.UserSolvedQuestions
                        .First(sq => sq.QuestionId == q.Id)
                        .SelectedAnswerIds ?? new List<long>()
                });

            var questionsWithCorrectAnswersAndUserAnswersWithCorrectness = questionsWithCorrectAnswersAndUserAnswers
                .Select(q => new {
                    q.Id,
                    q.Value,
                    Answers = q.Answers.Select(a => new {
                        a.Id,
                        a.Value,
                        a.IsCorrect,
                        Selected = q.UserSelectedAnswers.Contains(a.Id)
                    }),
                    AnsweredCorrectly = q.Answers
                        .Where(a => a.IsCorrect)
                        .Select(a => a.Id)
                        .OrderBy(a => a)
                        .SequenceEqual(q.UserSelectedAnswers.OrderBy(ua => ua))
                })
                .ToList();

            var correctlyAnsweredQuestionsCount = questionsWithCorrectAnswersAndUserAnswersWithCorrectness
                .Count(q => q.AnsweredCorrectly);

            var badlyAnsweredQuestionsCount = questionsWithCorrectAnswersAndUserAnswersWithCorrectness
                .Count(q => !q.AnsweredCorrectly);

            return new {
                Questions = questionsWithCorrectAnswersAndUserAnswersWithCorrectness,
                SolvingTime = solvedQuiz.SolvingTime,
                MaxSolvingTime = solvedQuiz.MaxSolvingTime,
                CorrectCount = correctlyAnsweredQuestionsCount,
                BadCount = badlyAnsweredQuestionsCount
            };
        }
    }
}
