using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using quizer_backend.Data;
using quizer_backend.Data.Entities;
using quizer_backend.Helpers;
using quizer_backend.Models;
using quizer_backend.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Repository.Interfaces;

namespace quizer_backend.Controllers {

    [Route("quizes")]
    public class QuizesController : QuizerApiControllerBase {

        private readonly Auth0ManagementFactory _auth0ManagementFactory;

        public QuizesController(IQuizerRepository repository, Auth0ManagementFactory auth0ManagementFactory) : base(repository) {
            _auth0ManagementFactory = auth0ManagementFactory;
        }

        private async Task<ManagementApiClient> GetManagementApiClientAsync()
            => await _auth0ManagementFactory.GetManagementApiClientAsync();


        // GETOS

        [HttpGet]
        public async Task<ActionResult> GetAllQuizes() {
            var quizes = Repository.GetAllQuizes(UserId(User));
            var quizesWithOwners = await IncludeOwnerNickNames(quizes);
            return ToJsonContentResult(quizesWithOwners);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetQuizByIdAsync(long id) {
            var quiz = await Repository.GetQuizByIdAsync(UserId(User), id);
            var quizWithOwner = await QuizItemWithOwnerNickName(quiz);

            return ToJsonContentResult(quizWithOwner);
        }

        [HttpGet("{id}/questions")]
        public async Task<ActionResult> GetQuizQuestionsByQuizId(long id, long? maxTime = null) {
            var questions = await Repository.GetQuizQuestionsByQuizIdAsync(UserId(User), id, maxTime: maxTime);

            questions = questions.Select(q => q.FlatVersionProps(maxTime));

            return ToJsonContentResult(questions.Select(q => new {
                q.Id,
                q.QuizId,
                q.Value,
                q.CreationTime
            }));
        }


        // POSTOS

        [HttpPost]
        public async Task<ActionResult> CreateQuizAsync(Quiz quiz) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var userId = UserId(User);
            quiz.OwnerId = userId;
            await Repository.AddQuizAsync(quiz);
            await Repository.AddQuizAccessAsync(new QuizAccess {
                Access = QuizAccessEnum.Owner,
                QuizId = quiz.Id,
                UserId = userId
            });
            return ToJsonContentResult(quiz);
        }

        [HttpPost("{id}/give-creator-access")]
        public async Task<ActionResult> GiveCreatorAccess(long id, string email) {
            var userId = UserId(User);
            var quiz = await Repository.GetQuizByIdAsync(userId, id);

            if (quiz == null || quiz.OwnerId != userId)
                return BadRequest();

            var client = await GetManagementApiClientAsync();
            var users = await client.Users.GetUsersByEmailAsync(email);

            if (users == null || users.Count == 0)
                return BadRequest();

            foreach (var user in users) {
                var access = new QuizAccess {
                    Access = QuizAccessEnum.Creator,
                    QuizId = quiz.Id,
                    UserId = user.UserId
                };
                await Repository.AddQuizAccessAsync(access);
            }

            return Ok();
        }


        // PUTOS

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateQuizNameAsync(long id, string name) {
            var quiz = await Repository.GetQuizByIdAsync(UserId(User), id);

            if (quiz == null)
                return NotFound();
            if (string.IsNullOrEmpty(name))
                return BadRequest();

            quiz.Name = name;
            await Repository.SaveAllAsync();
            //var quizversion = new QuizVersion {
            //    QuizId = id,
            //    Name = name
            //};
            //var result = await _repository.AddQuizVersionAsync(quizversion);
            //if (result) {
            //    quiz.Name = name;
            //    return ToJsonContentResult(quiz);
            //}

            return ToJsonContentResult(quiz);
        }


        // DELETOS

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteQuizAsync(long id) {
            bool deleted = await Repository.DeleteQuizByIdAsync(UserId(User), id);

            if (deleted)
                return Ok();

            return BadRequest();
        }


        // PRIVATE HELPEROS

        private async Task<IEnumerable<Quiz>> IncludeOwnerNickNames(IQueryable<Quiz> quizes) {
            var userIds = quizes.Select(q => q.OwnerId)
                                .Distinct();

            if (!userIds.Any())
                return quizes;

            var search = new GetUsersRequest {
                SearchEngine = "v3",
                Query = $"user_id: ({string.Join(" OR ", userIds)})"
            };
            var client = await GetManagementApiClientAsync();
            var owners = await client.Users.GetAllAsync(search);

            return from quiz in quizes
                   join owner in owners on quiz.OwnerId equals owner.UserId into users
                   from user in users.DefaultIfEmpty()
                   select quiz.IncludeOwnerNickNameInQuiz(user.NickName);
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