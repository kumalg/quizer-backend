using System;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using quizer_backend.Data.Entities;
using quizer_backend.Helpers;
using quizer_backend.Models;
using quizer_backend.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Repository;

namespace quizer_backend.Controllers {

    [Route("quizes")]
    public class QuizzesController : QuizerApiControllerBase {

        private readonly Auth0ManagementFactory _auth0ManagementFactory;
        private readonly QuizzesRepository _quizzesRepository;
        private readonly QuizAccessesRepository _quizAccessesRepository;
        private readonly QuestionsRepository _questionsRepository;

        public QuizzesController(
            QuizzesRepository quizzesRepository,
            QuestionsRepository questionsRepository,
            QuizAccessesRepository quizAccessesRepository,
            Auth0ManagementFactory auth0ManagementFactory
        ) {
            _auth0ManagementFactory = auth0ManagementFactory;
            _quizzesRepository = quizzesRepository;
            _questionsRepository = questionsRepository;
            _quizAccessesRepository = quizAccessesRepository;
        }

        private async Task<ManagementApiClient> GetManagementApiClientAsync()
            => await _auth0ManagementFactory.GetManagementApiClientAsync();


        // GETOS

        [HttpGet]
        public async Task<IActionResult> GetAllQuizzes() {
            var quizzes = _quizzesRepository.GetAllByUserId(UserId);
            var userId = UserId;

            foreach (var quiz in quizzes) {
                var userAccess = await _quizAccessesRepository.GetQuizAccessForUserAsync(userId, quiz.Id);
                quiz.IncludeAccess(userAccess.Access);
            }

            var quizzesWithOwners = await IncludeOwnerNickNames(quizzes);
            return Ok(quizzesWithOwners);
        }

        [HttpGet("{quizId}")]
        public async Task<IActionResult> GetQuizByIdAsync(long quizId) {
            var access = await _quizzesRepository.HaveReadAccessToQuizAsync(UserId, quizId);
            if (!access)
                return NotFound();

            var quiz = await _quizzesRepository.GetById(quizId);

            var userAccess = await _quizAccessesRepository.GetQuizAccessForUserAsync(UserId, quizId);
            quiz.IncludeAccess(userAccess.Access);

            var quizWithOwnerNickName = await QuizItemWithOwnerNickName(quiz);
            return Ok(quizWithOwnerNickName);
        }

        [HttpGet("{quizId}/questions")]
        public async Task<IActionResult> GetQuestionsByQuizId(long quizId, long? maxTime = null) {
            var access = await _quizzesRepository.HaveReadAccessToQuizAsync(UserId, quizId);
            if (!access)
                return NotFound();

            var questions = _questionsRepository
                .GetAllByQuizId(quizId, maxTime)
                .Include(q => q.Versions)
                .Select(q => q.FlatVersionProps(maxTime))
                .Select(q => new {
                    q.Id,
                    q.QuizId,
                    q.Value,
                    q.CreationTime
                });

            return Ok(questions);
        }

        [HttpGet("{id}/solve")]
        public async Task<IActionResult> GetRandomQuestionsWithQuizByQuizIdAsync(long id) {
            var userId = UserId;
            var haveReadAccess = await _quizzesRepository.HaveReadAccessToQuizAsync(userId, id);
            if (!haveReadAccess)
                return Forbid();

            var random = new Random();
            var quiz = await _quizzesRepository.GetById(id);
            await QuizItemWithOwnerNickName(quiz);

            var randomQuestionsQuery = _questionsRepository
                .GetAllByQuizId(id)
                .OrderBy(a => random.Next())
                .AsQueryable();

            var questionsInDatabase = await randomQuestionsQuery.CountAsync();

            if (quiz.QuestionsInSolvingQuiz != null) {
                randomQuestionsQuery = randomQuestionsQuery
                    .Take(quiz.QuestionsInSolvingQuiz.Value);
            }

            var randomQuestions = await randomQuestionsQuery
                .Include(q => q.Versions)
                .Include(q => q.Answers)
                .ThenInclude(a => a.Versions)
                .Select(q => new {
                    q.Id,
                    q.Versions
                        .OrderByDescending(v => v.CreationTime)
                        .FirstOrDefault()
                        .Value,
                    Answers = q.Answers
                        .Where(a => !a.IsDeleted)
                        .OrderBy(g => random.Next())
                        .Select(a => new {
                            a.Id,
                            a.Versions
                                .OrderByDescending(v => v.CreationTime)
                                .FirstOrDefault()
                                .Value
                        })
                })
                .ToListAsync();

            var creationTime = CurrentTime;

            return Ok(new {
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
            });
        }


        // POSTOS

        [HttpPost]
        public async Task<IActionResult> CreateQuizAsync(Quiz quiz) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var creationTime = CurrentTime;
            quiz.CreationTime = creationTime;
            quiz.LastModifiedTime = creationTime;
            quiz.OwnerId = UserId;

            await _quizzesRepository.Create(quiz);

            var access = new QuizAccess {
                Access = QuizAccessEnum.Owner,
                QuizId = quiz.Id,
                UserId = UserId
            };
            await _quizAccessesRepository.Create(access);

            return Created("quizes", quiz);
        }

        [HttpPost("{quizId}/give-creator-access")]
        public async Task<IActionResult> GiveCreatorAccess(long quizId, string email) {
            var haveOwnerAccess = await _quizzesRepository.HaveOwnerAccessToQuiz(UserId, quizId);
            if (!haveOwnerAccess)
                return NotFound();

            var client = await GetManagementApiClientAsync();
            var users = await client.Users.GetUsersByEmailAsync(email);

            if (users == null || users.Count == 0)
                return NotFound();

            foreach (var user in users) {
                var access = new QuizAccess {
                    Access = QuizAccessEnum.Creator,
                    QuizId = quizId,
                    UserId = user.UserId
                };
                await _quizAccessesRepository.Create(access);
            }

            return Ok();
        }

        [HttpPost("{quizId}/give-solver-access")]
        public async Task<IActionResult> GiveSolverAccess(long quizId, string email) {
            var haveWriteAccess = await _quizzesRepository.HaveWriteAccessToQuiz(UserId, quizId);
            if (!haveWriteAccess)
                return NotFound();

            var client = await GetManagementApiClientAsync();
            var users = await client.Users.GetUsersByEmailAsync(email);

            if (users == null || users.Count == 0)
                return NotFound();

            foreach (var user in users) {
                var access = new QuizAccess {
                    Access = QuizAccessEnum.Solver,
                    QuizId = quizId,
                    UserId = user.UserId
                };
                await _quizAccessesRepository.Create(access);
            }

            return Ok();
        }

        [HttpPost("{quizId}/stop-co-creating")]
        public async Task<IActionResult> StopCoCreating(long quizId) {
            var userId = UserId;

            var access = await _quizAccessesRepository.GetQuizAccessForUserAsync(userId, quizId);
            if (access == null || access.Access != QuizAccessEnum.Creator)
                return BadRequest();

            access.Access = QuizAccessEnum.Solver;

            var result = await _quizAccessesRepository.Update(access.Id, access);
            if (result)
                return Ok();

            return BadRequest();
        }

        [HttpPost("{quizId}/leave")]
        public async Task<IActionResult> Leave(long quizId) {
            var result = await _quizAccessesRepository.Delete(UserId, quizId);
            if (result)
                return Ok();

            return BadRequest();
        }


        // PUTOS

        //[HttpPut("{quizId}")]
        //public async Task<IActionResult> UpdateQuizNameAsync(long quizId, string name) {
        //    if (string.IsNullOrEmpty(name))
        //        return BadRequest("name cannot be empty");

        //    var haveOwnerAccess = await _quizzesRepository.HaveOwnerAccessToQuiz(UserId, quizId);
        //    if (!haveOwnerAccess)
        //        return NotFound();

        //    var quiz = await _quizzesRepository.GetById(quizId);
        //    if (quiz == null)
        //        return NotFound();

        //    quiz.Name = name;

        //    await _quizzesRepository.Update(quiz.Id, quiz);
        //    return Ok(quiz);
        //}

        [HttpPut("{quizId}")]
        public async Task<IActionResult> UpdateQuizAsync(long quizId, Quiz newQuiz) {
            if (string.IsNullOrEmpty(newQuiz.Name))
                return BadRequest("name cannot be empty");

            var haveOwnerAccess = await _quizzesRepository.HaveOwnerAccessToQuiz(UserId, quizId);
            if (!haveOwnerAccess)
                return NotFound();

            var quiz = await _quizzesRepository.GetById(quizId);
            if (quiz == null)
                return NotFound();

            quiz.Name = newQuiz.Name;
            quiz.QuestionsInSolvingQuiz = newQuiz.QuestionsInSolvingQuiz;
            quiz.MinutesInSolvingQuiz = newQuiz.MinutesInSolvingQuiz;

            await _quizzesRepository.Update(quiz.Id, quiz);
            return Ok(quiz);
        }


        // DELETOS

        [HttpDelete("{quizId}")]
        public async Task<IActionResult> DeleteQuizAsync(long quizId) {
            var haveOwnerAccess = await _quizzesRepository.HaveOwnerAccessToQuiz(UserId, quizId);
            if (!haveOwnerAccess)
                return NotFound();

            var deleted = await _quizzesRepository.Delete(quizId);
            if (!deleted)
                return NotFound();

            return Ok();
        }


        // PRIVATE HELPEROS

        private async Task<IEnumerable<Quiz>> IncludeOwnerNickNames(IQueryable<Quiz> quizzes) {
            var userIds = quizzes
                .Select(q => q.OwnerId)
                .Distinct();

            if (!userIds.Any())
                return quizzes;

            var search = new GetUsersRequest {
                SearchEngine = "v3",
                Query = $"user_id: ({string.Join(" OR ", userIds)})"
            };
            var client = await GetManagementApiClientAsync();
            var owners = await client.Users.GetAllAsync(search);

            return from quiz in quizzes
                   join owner in owners on quiz.OwnerId equals owner.UserId into users
                   from user in users.DefaultIfEmpty()
                   select quiz.IncludeOwner(user);
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