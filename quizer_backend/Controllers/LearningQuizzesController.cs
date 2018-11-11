using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using quizer_backend.Data.Entities;
using quizer_backend.Data.Entities.LearningQuiz;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Repository;
using quizer_backend.Helpers;
using quizer_backend.Models;
using quizer_backend.Services;

namespace quizer_backend.Controllers {

    [Route("learning-quizes")]
    public class LearningQuizzesController : QuizerApiControllerBase
    {
        private readonly Auth0ManagementFactory _auth0ManagementFactory;
        private readonly AnonymousUsersRepository _anonymousUsersRepository;
        private readonly UserSettingsRepository _userSettingsRepository;
        private readonly QuizzesRepository _quizzesRepository;
        private readonly QuizAccessesRepository _quizAccessesRepository;
        private readonly QuestionsRepository _questionsRepository;
        private readonly AnswersRepository _answersRepository;
        private readonly LearningQuizzesRepository _learningQuizzesRepository;
        private readonly LearningQuizQuestionsRepository _learningQuizQuestionsRepository;

        public LearningQuizzesController(
            IConfiguration configuration,
            QuizzesRepository quizzesRepository,
            AnonymousUsersRepository anonymousUsersRepository,
            UserSettingsRepository userSettingsRepository,
            QuizAccessesRepository quizAccessesRepository,
            QuestionsRepository questionsRepository,
            AnswersRepository answersRepository,
            LearningQuizzesRepository learningQuizzesRepository,
            LearningQuizQuestionsRepository learningQuizQuestionsRepository,
            Auth0ManagementFactory auth0ManagementFactory
        ) {
            _auth0ManagementFactory = auth0ManagementFactory;
            _anonymousUsersRepository = anonymousUsersRepository;
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

        [AllowAnonymous]
        [HttpGet("{quizId}/anonymous-learning-quiz-instances")]
        public async Task<IActionResult> GetAllLearningQuizInstancesOfQuiz(Guid quizId) {
            if (UserId != null)
                return BadRequest("You are logged in");

            var anonymousUserId = await GetAnonymousUserId(Request);
            if (anonymousUserId == null)
                return Ok(new long[] { });

            var instances = await _learningQuizzesRepository
                .GetAll()
                .Where(l => !l.IsFinished)
                .Where(l => l.UserId == anonymousUserId)
                .Where(l => l.QuizId == quizId)
                .ToListAsync();

            return Ok(instances);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLearningQuizzesAsync() {
            var learningQuizzes = await _learningQuizzesRepository
                .GetAllByUserId(UserId)
                .Include(q => q.Quiz)
                .ToListAsync();

            foreach (var learningQuiz in learningQuizzes) {
                if (learningQuiz.QuizId == null) continue;
                var access = await _quizAccessesRepository.GetQuizAccessForUserAsync(UserId, learningQuiz.QuizId.Value);
                learningQuiz.Quiz.IncludeAccess(access.Access);
            }

            var learningQuizzesWithOwners = await IncludeOwnerNickNames(learningQuizzes);
            return Ok(learningQuizzesWithOwners);
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetAllCurrentLearningQuizzesAsync() {
            var learningQuizzes = await _learningQuizzesRepository
                .GetAllByUserId(UserId)
                .Where(q => !q.IsFinished)
                .Include(q => q.Quiz)
                .ToListAsync();

            for (var i = learningQuizzes.Count - 1; i >= 0; i--) {
                if (learningQuizzes[i].QuizId == null)
                    continue;

                var access = await _quizAccessesRepository.GetQuizAccessForUserAsync(UserId, learningQuizzes[i].QuizId.Value);
                if (access == null || access.Access == QuizAccessEnum.None) {
                    learningQuizzes.RemoveAt(i);
                }
                else {
                    learningQuizzes[i].Quiz.IncludeAccess(access.Access);
                }
            }

            var learningQuizzesWithOwners = await IncludeOwnerNickNames(learningQuizzes);
            return Ok(learningQuizzesWithOwners);
        }

        [HttpGet("finished")]
        public async Task<IActionResult> GetAllFinishedLearningQuizzesAsync() {
            var learningQuizzes = await _learningQuizzesRepository
                .GetAllByUserId(UserId)
                .Where(q => q.IsFinished)
                .Include(q => q.Quiz)
                .ToListAsync();

            for (var i = learningQuizzes.Count - 1; i >= 0; i--) {
                if (learningQuizzes[i].QuizId == null)
                    continue;

                var access = await _quizAccessesRepository.GetQuizAccessForUserAsync(UserId, learningQuizzes[i].QuizId.Value);
                if (access == null || access.Access == QuizAccessEnum.None) {
                    learningQuizzes.RemoveAt(i);
                }
                else {
                    learningQuizzes[i].Quiz.IncludeAccess(access.Access);
                }
            }

            var learningQuizzesWithOwners = await IncludeOwnerNickNames(learningQuizzes);
            return Ok(learningQuizzesWithOwners);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLearningQuizByIdAsync(long id) {
            var userId = UserId ?? await GetAnonymousUserId(Request);

            var learningQuiz = await _learningQuizzesRepository
                .GetAllByUserId(userId)
                .Where(q => q.Id == id)
                .Include(q => q.Quiz)
                .SingleOrDefaultAsync();

            if (learningQuiz?.QuizId == null)
                return NotFound();

            var isPublic = await _quizzesRepository.IsPublicAsync(learningQuiz.QuizId.Value);
            if (!isPublic) {
                var access = await _quizzesRepository.HaveReadAccessToQuizAsync(userId, learningQuiz.QuizId.Value);
                if (!access)
                    return Forbid();
            }

            await QuizItemWithOwnerNickName(learningQuiz.Quiz);
            return Ok(learningQuiz);
        }

        [AllowAnonymous]
        [HttpGet("{id}/next-question")]
        public async Task<IActionResult> GetNextQuestionAsync(long id) {
            var userId = UserId ?? await GetAnonymousUserId(Request);

            var learningQuiz = await _learningQuizzesRepository.GetById(id);
            if (learningQuiz?.QuizId == null || learningQuiz.UserId != userId)
                return BadRequest();
            
            var isPublic = await _quizzesRepository.IsPublicAsync(learningQuiz.QuizId.Value);
            if (!isPublic) {
                var access = await _quizzesRepository.HaveReadAccessToQuizAsync(userId, learningQuiz.QuizId.Value);
                if (!access)
                    return Forbid();
            }

            var maxVersionTime = learningQuiz.CreationTime;
            var reoccurrences = _learningQuizQuestionsRepository.GetAllByLearningQuizId(learningQuiz.Id);

            if (reoccurrences == null)
                return BadRequest();

            var remainingReoccurrences = reoccurrences.Where(q => q.Reoccurrences > 0);

            if (!remainingReoccurrences.Any()) {
                return Ok(new { IsFinished = true });
            }

            var randomQuestionReoccurrences = remainingReoccurrences.RandomElement(p => true);

            var randomQuestion = await _questionsRepository
                .GetAll()
                .Where(q => q.Id == randomQuestionReoccurrences.QuestionId)
                .Where(q => q.CreationTime <= maxVersionTime)
                .Include(q => q.Versions)
                .SingleOrDefaultAsync();

            randomQuestion.FlatVersionProps(maxVersionTime);

            var answers = await _answersRepository
                .GetAllByQuestionId(randomQuestion.Id, maxVersionTime, true) //TODO MIN VERSION TIME
                .Include(a => a.Versions)
                .Select(a => new {
                    a.Id,
                    a.Versions
                        .Where(b => b.CreationTime <= maxVersionTime)
                        .OrderByDescending(b => b.CreationTime)
                        .FirstOrDefault()
                        .Value
                })
                .ToListAsync();

            return Ok(new {
                IsFinished = false,
                randomQuestionReoccurrences.Reoccurrences,
                Question = randomQuestion,
                Answers = answers
            });
        }


        // POSTOS
        
        private async Task<IActionResult> NewAuthorizedLearningQuiz(Guid quizId, string userId) {
            var creationTime = CurrentTime;

            var questionsQuery = _questionsRepository
                .GetAllByQuizId(quizId)
                .Where(q => q.CreationTime <= creationTime);

            var learningQuiz = new LearningQuiz {
                CreationTime = creationTime,
                UserId = userId,
                QuizId = quizId,
                NumberOfQuestions = await questionsQuery.CountAsync()
            };

            var settings = await _userSettingsRepository.GetByIdOrDefault(userId);
            var questions = questionsQuery
                .Select(q => new LearningQuizQuestion {
                    QuestionId = q.Id,
                    Reoccurrences = settings.ReoccurrencesOnStart
                });

            await _learningQuizzesRepository.Create(learningQuiz, questions);

            return Ok(learningQuiz);
        }

        private async Task<string> GetAnonymousUserId(HttpRequest request) {
            var tokenString = request.Cookies["anonymous_user_id_token"];

            if (string.IsNullOrEmpty(tokenString))
                return null;

            var userId = AnonymousUsersService.GetUserId(tokenString);
            if (string.IsNullOrEmpty(userId))
                return "";
            var userGuid = Guid.Parse(userId);
            var exist = await _anonymousUsersRepository.ExistAsync(userGuid);
            return exist ? userId : "";
        }

        private async Task<string> GenerateAnonymousUserId(HttpResponse response) {
            var user = new AnonymousUser();
            await _anonymousUsersRepository.Create(user);
            var userId = user.Id.ToString();
            var token = AnonymousUsersService.GenerateTokenFromUserId(userId);
            response.Cookies.Append("anonymous_user_id_token", token, new CookieOptions {
                HttpOnly = true
            });
            return userId;
        }

        // POSTOS

        [AllowAnonymous]
        [HttpPost("{quizId}")]
        public async Task<IActionResult> CreateLearningQuiz(Guid quizId) {
            var userId = UserId ?? await GetAnonymousUserId(Request) ?? await GenerateAnonymousUserId(Response);

            var isPublic = await _quizzesRepository.IsPublicAsync(quizId);
            if (!isPublic) {
                var access = await _quizzesRepository.HaveReadAccessToQuizAsync(userId, quizId);
                if (!access)
                    return NotFound();
            }

            return await NewAuthorizedLearningQuiz(quizId, userId);
        }

        [AllowAnonymous]
        [HttpPost("{learningQuizId}/answer")]
        public async Task<IActionResult> AnswerTheQuestion(long learningQuizId, LearningQuizUserAnswer userAnswer) {
            var userId = UserId ?? await GetAnonymousUserId(Request);

            var learningQuiz = await _learningQuizzesRepository.GetById(learningQuizId);
            if (learningQuiz?.QuizId == null || learningQuiz.UserId != userId)
                return BadRequest();

            var questionReoccurrences = await _learningQuizQuestionsRepository
                .GetAllByLearningQuizId(learningQuizId: learningQuizId, asNoTracking: false)
                .Where(q => q.QuestionId == userAnswer.QuizQuestionId)
                .SingleOrDefaultAsync();

            if (questionReoccurrences == null)
                return BadRequest();

            var correctAnswers = await _answersRepository
                .GetAllByQuestionId(userAnswer.QuizQuestionId, learningQuiz.CreationTime, true)
                .Include(a => a.Versions)
                .Select(a => new {
                    a.Id,
                    a.Versions
                        .Where(b => b.CreationTime <= learningQuiz.CreationTime)
                        .OrderByDescending(b => b.CreationTime)
                        .FirstOrDefault()
                        .IsCorrect
                })
                .Where(a => a.IsCorrect)
                .Select(i => i.Id)
                .OrderBy(i => i)
                .ToListAsync();

            var selectedAnswers = userAnswer.SelectedAnswers
                .OrderBy(i => i)
                .ToList();

            var settings = await _userSettingsRepository.GetByIdOrDefault(userId);
            var isCorrect = correctAnswers.SequenceEqual(selectedAnswers);
            if (isCorrect) {
                learningQuiz.NumberOfCorrectAnswers = learningQuiz.NumberOfCorrectAnswers + 1;
                questionReoccurrences.Reoccurrences = questionReoccurrences.Reoccurrences - 1;
                if (questionReoccurrences.Reoccurrences == 0) {
                    learningQuiz.NumberOfLearnedQuestions = learningQuiz.NumberOfLearnedQuestions + 1;
                }
            }
            else {
                learningQuiz.NumberOfBadAnswers = learningQuiz.NumberOfBadAnswers + 1;
                questionReoccurrences.Reoccurrences = questionReoccurrences.Reoccurrences + settings.ReoccurrencesIfBad;
                if (questionReoccurrences.Reoccurrences > settings.MaxReoccurrences)
                    questionReoccurrences.Reoccurrences = settings.MaxReoccurrences;
            }

            await _learningQuizQuestionsRepository.Update(questionReoccurrences.Id, questionReoccurrences);
            await _learningQuizzesRepository.Update(learningQuiz.Id, learningQuiz);

            var isUnfinished = await _learningQuizQuestionsRepository
                .GetAllByLearningQuizId(learningQuizId)
                .Where(q => q.Reoccurrences > 0)
                .AnyAsync();

            if (!isUnfinished)
                await _learningQuizzesRepository.SetAsFinished(learningQuizId, CurrentTime);

            return Ok(new {
                IsFinished = !isUnfinished,
                IsCorrect = isCorrect,
                CorrectAnswers = correctAnswers,
                SelectedAnswers = selectedAnswers,
                questionReoccurrences.Reoccurrences,
                LearningQuiz = learningQuiz
            });
        }


        // DELETOS

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuizAsync(long id) {
            var haveOwnerAccess = await _learningQuizzesRepository
                .GetAllByUserId(UserId)
                .Where(l => l.Id == id)
                .AnyAsync();

            if (!haveOwnerAccess)
                return NotFound();

            var deleted = await _learningQuizzesRepository.Delete(id);
            if (!deleted)
                return BadRequest();

            return Ok();
        }


        // PRIVATE HELPEROS

        private async Task<IEnumerable<LearningQuiz>> IncludeOwnerNickNames(IList<LearningQuiz> quizzes) {
            var userIds = quizzes
                .Where(q => q.QuizId != null)
                .Select(q => q.Quiz.OwnerId)
                .Distinct()
                .ToList();

            if (!userIds.Any())
                return quizzes;

            var search = new GetUsersRequest {
                SearchEngine = "v3",
                Query = $"user_id: ({string.Join(" OR ", userIds)})"
            };
            var client = await GetManagementApiClientAsync();
            var owners = await client.Users.GetAllAsync(search);

            var yco = from quiz in quizzes
                      where quiz.QuizId != null
                      join owner in owners on quiz.Quiz.OwnerId equals owner.UserId into users
                      from user in users.DefaultIfEmpty()
                      select quiz.IncludeOwnerInQuiz(user);

            return yco.Concat(quizzes.Where(q => q.QuizId == null));
        }

        private async Task<Quiz> QuizItemWithOwnerNickName(Quiz quiz) {
            var client = await _auth0ManagementFactory.GetManagementApiClientAsync();
            var owner = await client.Users.GetAsync(quiz.OwnerId);

            if (owner != null)
                quiz.IncludeOwner(owner);

            return quiz;
        }
    }
}