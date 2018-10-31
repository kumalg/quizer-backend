//using Auth0.ManagementApi;
//using Auth0.ManagementApi.Models;
//using Microsoft.AspNetCore.Mvc;
//using quizer_backend.Data;
//using quizer_backend.Helpers;
//using quizer_backend.Services;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using quizer_backend.Data.Entities.QuizObject;
//using quizer_backend.Data.Entities.SolvingQuiz;
//using quizer_backend.Data.Repository.Interfaces;
//using quizer_backend.Models;

//namespace quizer_backend.Controllers {

//    [Route("solving-quizes")]
//    public class SolvingQuizzesController : QuizerApiControllerBase {

//        private readonly Auth0ManagementFactory _auth0ManagementFactory;

//        public SolvingQuizzesController(IQuizerRepository repository, Auth0ManagementFactory auth0ManagementFactory) : base(repository) {
//            _auth0ManagementFactory = auth0ManagementFactory;
//        }

//        private async Task<ManagementApiClient> GetManagementApiClientAsync()
//            => await _auth0ManagementFactory.GetManagementApiClientAsync();


//        // GETOS

//        [HttpGet]
//        public async Task<ActionResult> GetAllSolvingQuizesAsync() {
//            var solvingQuizes = await Repository.GetAllSolvingQuizes(UserId(User));
//            var solvingQuizesWithOwners = await IncludeOwnerNickNames(solvingQuizes);
//            return ToJsonContentResult(solvingQuizesWithOwners);
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult> GetSolvingQuizByIdAsync(long id) {
//            var solvingQuiz = await Repository.GetSolvingQuizByIdAsync(UserId(User), id);
//            await QuizItemWithOwnerNickName(solvingQuiz.Quiz);

//            return ToJsonContentResult(solvingQuiz);
//        }

//        [HttpGet("{id}/next-question")]
//        public async Task<ActionResult> GetNextQuestionAsync(long id) {
//            var userId = UserId(User);

//            var solvingQuiz = await Repository.GetSolvingQuizByIdAsync(userId, id);
//            if (solvingQuiz?.QuizId == null)
//                return BadRequest();

//            var questions = await Repository.GetQuestionsByQuizIdAsync(userId, solvingQuiz.QuizId ?? -1, maxTime: solvingQuiz.CreationTime);
//            var finishedQuestions = await Repository.GetSolvingQuizFinishedQuestions(userId, solvingQuiz.Id);
//            var remainingQuestions = questions.AsEnumerable().Except(finishedQuestions.Select(f => f.Question));
            
//            var randomQuestion = remainingQuestions.AsQueryable()
//                                                   .RandomElement(p => true)
//                                                   .FlatVersionProps(solvingQuiz.CreationTime);

//            return ToJsonContentResult(randomQuestion);
//        }


//        // POSTOS

//        [HttpPost("{quizId}")]
//        public async Task<ActionResult> CreateSolvingQuiz(long quizId) {
//            if (!ModelState.IsValid) {
//                return BadRequest(ModelState);
//            }
//            var solvingQuiz = new SolvingQuiz {
//                UserId = UserId(User),
//                QuizId = quizId
//            };
//            await Repository.AddSolvingQuizAsync(solvingQuiz);
//            return ToJsonContentResult(solvingQuiz);
//        }

//        [HttpPost("{solvingQuizId}/answer")]
//        public async Task<ActionResult> AnswerTheQuestion(long solvingQuizId, SolvingQuizAnswer answer) {
//            var question = await Repository.GetQuestionByIdAsync(UserId(User), answer.QuestionId);
//            var answers = question.Answers;

//            var correctAnswers = answers.Where(i => i.IsCorrect);

//            //throw NotImplementedException();

//            return Ok();
//        }


//        //// PUTOS



//        // DELETOS

//        [HttpDelete("{id}")]
//        public async Task<ActionResult> DeleteSolvingQuizAsync(long id) {
//            var deleted = await Repository.DeleteSolvingQuizAsync(UserId(User), id);
//            if (deleted) return Ok();
//            return BadRequest();
//        }


//        // PRIVATE HELPEROS

//        private async Task<IEnumerable<SolvingQuiz>> IncludeOwnerNickNames(IQueryable<SolvingQuiz> quizes) {
//            var userIds = quizes.Select(q => q.Quiz.OwnerId)
//                                .Distinct();

//            if (!userIds.Any())
//                return quizes;

//            var search = new GetUsersRequest {
//                SearchEngine = "v3",
//                Query = $"user_id: ({string.Join(" OR ", userIds)})"
//            };
//            var client = await GetManagementApiClientAsync();
//            var owners = await client.Users.GetAllAsync(search);

//            return from quiz in quizes
//                   join owner in owners on quiz.Quiz.OwnerId equals owner.UserId into users
//                   from user in users.DefaultIfEmpty()
//                   select quiz.IncludeOwnerNickName(user.NickName);
//        }

//        private async Task<Quiz> QuizItemWithOwnerNickName(Quiz quiz) {
//            var client = await _auth0ManagementFactory.GetManagementApiClientAsync();
//            var owner = await client.Users.GetAsync(quiz.OwnerId);

//            if (owner != null)
//                quiz.OwnerNickName = owner.NickName;

//            return quiz;
//        }
//    }
//}