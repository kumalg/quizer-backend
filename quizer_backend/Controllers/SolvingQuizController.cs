using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using quizer_backend.Data;
using quizer_backend.Data.Entities;
using quizer_backend.Helpers;
using quizer_backend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace quizer_backend.Controllers {

    [Route("solving-quizes")]
    public class SolvingQuizController : QuizerApiControllerBase {

        private readonly Auth0ManagementFactory _auth0ManagementFactory;

        public SolvingQuizController(IQuizerRepository repository, Auth0ManagementFactory auth0ManagementFactory) : base(repository) {
            _auth0ManagementFactory = auth0ManagementFactory;
        }

        private async Task<ManagementApiClient> GetManagementApiClientAsync()
            => await _auth0ManagementFactory.GetManagementApiClientAsync();


        // GETOS

        [HttpGet]
        public async Task<ActionResult> GetAllSolvingQuizesAsync() {
            var solvingQuizes = await _repository.GetAllSolvingQuizes(UserId(User));
            var solvingQuizesWithOwners = await IncludeOwnerNickNames(solvingQuizes);
            return ToJsonContentResult(solvingQuizesWithOwners);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetSolvingQuizByIdAsync(long id) {
            var solvingQuiz = await _repository.GetSolvingQuizByIdAsync(UserId(User), id);
            await QuizItemWithOwnerNickName(solvingQuiz.Quiz);

            return ToJsonContentResult(solvingQuiz);
        }

        [HttpGet("{id}/next-question")]
        public async Task<ActionResult> GetNextQuestionAsync(long id) {
            var userId = UserId(User);

            var solvingQuiz = await _repository.GetSolvingQuizByIdAsync(userId, id);
            if (solvingQuiz == null || solvingQuiz.QuizId == null)
                return BadRequest();

            var questions = await _repository.GetQuizQuestionsByQuizIdAsync(userId, solvingQuiz.QuizId ?? -1, maxTime: solvingQuiz.CreationTime);
            List<SolvingQuizFinishedQuestion> finishedQuestions = await _repository.GetSolvingQuizFinishedQuestions(userId, solvingQuiz.Id);
            var remainingQuestions = questions.Except(finishedQuestions.Select(f => f.QuizQuestion));

            var rand = new Random();
            var selectedQuestion = remainingQuestions.RandomElement(p => true).FlatVersionProps(solvingQuiz.CreationTime);

            return ToJsonContentResult(new {
                selectedQuestion.Id,
                selectedQuestion.QuizId,
                selectedQuestion.Value,
                selectedQuestion.CreationTime
            });
        }


        // POSTOS

        [HttpPost("{quizId}")]
        public async Task<ActionResult> CreateSolvingQuiz(long quizId) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var solvingQuiz = new SolvingQuiz {
                UserId = UserId(User),
                QuizId = quizId
            };
            await _repository.AddSolvingQuizAsync(solvingQuiz);
            return ToJsonContentResult(solvingQuiz);
        }


        //// PUTOS
        


        // DELETOS

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSolvingQuizAsync(long id) {
            bool deleted = await _repository.DeleteSolvingQuizAsync(UserId(User), id);
            if (deleted) return Ok();
            return BadRequest();
        }


        // PRIVATE HELPEROS

        private async Task<IEnumerable<SolvingQuiz>> IncludeOwnerNickNames(IEnumerable<SolvingQuiz> quizes) {
            var userIds = quizes.Select(q => q.Quiz.OwnerId)
                                .Distinct();

            if (userIds == null || !userIds.Any())
                return quizes;

            var search = new GetUsersRequest {
                SearchEngine = "v3",
                Query = $"user_id: ({string.Join(" OR ", userIds)})"
            };
            var client = await GetManagementApiClientAsync();
            var owners = await client.Users.GetAllAsync(search);

            return from quiz in quizes
                   join owner in owners on quiz.Quiz.OwnerId equals owner.UserId into users
                   from user in users.DefaultIfEmpty()
                   select quiz.IncludeOwnerNickNameInQuiz(user?.NickName);
        }

        private async Task<Quiz> QuizItemWithOwnerNickName(Quiz quiz) {
            var client = await _auth0ManagementFactory.GetManagementApiClientAsync();
            var owner = await client.Users.GetAsync(quiz.OwnerId);

            if (owner != null)
                quiz.OwnerNickName = owner.NickName;

            return quiz;
        }
    }
}