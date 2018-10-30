using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using quizer_backend.Data.Entities.LearningQuiz;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Repository.Interfaces;
using quizer_backend.Helpers;
using quizer_backend.Models;
using quizer_backend.Services;

namespace quizer_backend.Controllers {

    [Route("learning-quizes")]
    public class LearningQuizzesController : QuizerApiControllerBase {

        private readonly Auth0ManagementFactory _auth0ManagementFactory;
        private readonly ILearningQuizzesRepository _learningQuizzesRepository;

        public LearningQuizzesController(IQuizerRepository repository, ILearningQuizzesRepository learningQuizzesRepository, Auth0ManagementFactory auth0ManagementFactory)
            : base(repository) {
            _auth0ManagementFactory = auth0ManagementFactory;
            _learningQuizzesRepository = learningQuizzesRepository;
        }

        private async Task<ManagementApiClient> GetManagementApiClientAsync()
            => await _auth0ManagementFactory.GetManagementApiClientAsync();


        // GETOS

        [HttpGet]
        public async Task<ActionResult> GetAllLearningQuizzesAsync() {
            var learningQuizzes = await _learningQuizzesRepository.GetAllLearningQuizes(UserId(User), true);
            var learningQuizzesWithOwners = await IncludeOwnerNickNames(learningQuizzes);
            return ToJsonContentResult(learningQuizzesWithOwners);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetLearningQuizByIdAsync(long id) {
            var learningQuiz = await _learningQuizzesRepository.GetLearningQuizByIdAsync(UserId(User), id, true);
            if (learningQuiz == null)
                return NotFound();

            await QuizItemWithOwnerNickName(learningQuiz.Quiz);
            return ToJsonContentResult(learningQuiz);
        }

        [HttpGet("{id}/next-question")]
        public async Task<ActionResult> GetNextQuestionAsync(long id) {
            var userId = UserId(User);

            var learningQuiz = await _learningQuizzesRepository.GetLearningQuizByIdAsync(userId, id);
            if (learningQuiz?.QuizId == null)
                return BadRequest();

            var maxVersionTime = learningQuiz.CreationTime;
            var reoccurrences = _learningQuizzesRepository.GetLearningQuizQuestionsReoccurrences(learningQuiz.Id);

            if (reoccurrences == null)
                return BadRequest();

            var remainingReoccurrences = reoccurrences.Where(q => q.Reoccurrences > 0);
            
            if (!remainingReoccurrences.Any()) {
                return ToJsonContentResult(new {
                    IsFinished = true
                });
            }

            var randomQuestionReoccurrences = remainingReoccurrences.RandomElement(p => true);

            var randomQuestion = await Repository.GetQuizQuestionByIdAsync(userId, randomQuestionReoccurrences.QuizQuestionId, maxVersionTime);

            randomQuestion.FlatVersionProps(maxVersionTime);

            var answers = await Repository.GetQuizQuestionAnswersByQuizQuestionIdAsync(userId, randomQuestion.Id, maxVersionTime);

            foreach (var answer in answers)
                answer.FlatVersionProps(maxVersionTime);

            return ToJsonContentResult(new {
                IsFinished = false,
                Reoccurrences = randomQuestionReoccurrences.Reoccurrences,
                Question = randomQuestion,
                Answers = answers.Select(a => new {
                    a.Id,
                    a.Value
                })
            });
        }


        // POSTOS

        [HttpPost("{quizId}")]
        public async Task<ActionResult> CreateLearningQuiz(long quizId) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var learningQuiz = new LearningQuiz {
                UserId = UserId(User),
                QuizId = quizId
            };
            await _learningQuizzesRepository.AddLearningQuizAsync(learningQuiz);
            return ToJsonContentResult(learningQuiz);
        }

        [HttpPost("{learningQuizId}/answer")]
        public async Task<ActionResult> AnswerTheQuestion(long learningQuizId, LearningQuizUserAnswer userAnswer) {
            var userId = UserId(User);

            var learningQuiz = await _learningQuizzesRepository.GetLearningQuizByIdAsync(userId, learningQuizId);
            if (learningQuiz == null)
                return BadRequest();

            var answers = await Repository.GetQuizQuestionAnswersByQuizQuestionIdAsync(userId, userAnswer.QuizQuestionId, learningQuiz.CreationTime);
            if (answers == null)
                return BadRequest();

            var questionReoccurrences = await _learningQuizzesRepository.GetLearningQuizQuestionReoccurrences(learningQuizId, userAnswer.QuizQuestionId);
            if (questionReoccurrences == null)
                return BadRequest();
            
            var correctAnswers = answers.AsEnumerable().Select(i => i.FlatVersionProps(learningQuiz.CreationTime)).Where(i => i.IsCorrect).Select(i => i.Id).OrderBy(i => i).ToList();
            var selectedAnswers = userAnswer.SelectedAnswers.OrderBy(i => i).ToList();

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
                questionReoccurrences.Reoccurrences = questionReoccurrences.Reoccurrences + 1;
            }
            await Repository.SaveAllAsync();
            var isFinished = await _learningQuizzesRepository.IsLearningQuizFinished(userId, learningQuizId);
            if (isFinished.Value)
                await _learningQuizzesRepository.FinishLearningQuiz(userId, learningQuizId);

            return ToJsonContentResult(new {
                IsFinished = isFinished,
                IsCorrect = isCorrect,
                CorrectAnswers = correctAnswers,
                SelectedAnswers = selectedAnswers,
                Reoccurrences = questionReoccurrences.Reoccurrences,
                LearningQuiz = learningQuiz
            });
        }


        // PRIVATE HELPEROS

        private async Task<IEnumerable<LearningQuiz>> IncludeOwnerNickNames(IQueryable<LearningQuiz> quizes) {
            var userIds = quizes.Select(q => q.Quiz.OwnerId)
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
                join owner in owners on quiz.Quiz.OwnerId equals owner.UserId into users
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