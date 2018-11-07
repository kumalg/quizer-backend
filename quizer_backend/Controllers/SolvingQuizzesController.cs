using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth0.ManagementApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Repository;
using quizer_backend.Models;
using quizer_backend.Services;

namespace quizer_backend.Controllers {

    [Route("solving-quizzes")]
    public class SolvingQuizzesController : QuizerApiControllerBase {

        private readonly Auth0ManagementFactory _auth0ManagementFactory;
        private readonly UserSettingsRepository _userSettingsRepository;
        private readonly QuizzesRepository _quizzesRepository;
        private readonly QuizAccessesRepository _quizAccessesRepository;
        private readonly QuestionsRepository _questionsRepository;
        private readonly AnswersRepository _answersRepository;
        private readonly LearningQuizzesRepository _learningQuizzesRepository;
        private readonly LearningQuizQuestionsRepository _learningQuizQuestionsRepository;

        public SolvingQuizzesController(
            QuizzesRepository quizzesRepository,
            UserSettingsRepository userSettingsRepository,
            QuizAccessesRepository quizAccessesRepository,
            QuestionsRepository questionsRepository,
            AnswersRepository answersRepository,
            LearningQuizzesRepository learningQuizzesRepository,
            LearningQuizQuestionsRepository learningQuizQuestionsRepository,
            Auth0ManagementFactory auth0ManagementFactory
        ) {
            _auth0ManagementFactory = auth0ManagementFactory;
            _userSettingsRepository = userSettingsRepository;
            _quizzesRepository = quizzesRepository;
            _quizAccessesRepository = quizAccessesRepository;
            _questionsRepository = questionsRepository;
            _answersRepository = answersRepository;
            _learningQuizzesRepository = learningQuizzesRepository;
            _learningQuizQuestionsRepository = learningQuizQuestionsRepository;
        }

        private async Task<ManagementApiClient> GetManagementApiClientAsync()
            => await _auth0ManagementFactory.GetManagementApiClientAsync();


        // GETOS



        // POSTOS

        [HttpPost]
        public async Task<IActionResult> PostSolvedQuiz(UserSolvedQuiz solvedQuiz) {
            var haveReadAccess = await _quizzesRepository.HaveReadAccessToQuizAsync(UserId, solvedQuiz.QuizId);
            if (!haveReadAccess)
                return Forbid();

            var quiz = _quizzesRepository.GetById(solvedQuiz.QuizId); // nie wiem po co;
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
                return BadRequest();

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

            return Ok(new {
                Questions = questionsWithCorrectAnswersAndUserAnswersWithCorrectness,
                SolvingTime = solvedQuiz.SolvingTime,
                MaxSolvingTime = solvedQuiz.MaxSolvingTime,
                CorrectCount = correctlyAnsweredQuestionsCount,
                BadCount = badlyAnsweredQuestionsCount
            });
        }


        //// PUTOS



        // DELETOS



        // PRIVATE HELPEROS

    }
}