using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using quizer_backend.Data.Entities;
using quizer_backend.Data.Services;
using quizer_backend.Models;
using quizer_backend.Services;

namespace quizer_backend.Controllers {

    [Route("learning-quizes")]
    public class LearningQuizzesController : QuizerApiControllerBase {

        private readonly LearningQuizzesService _learningQuizzesService;
        private readonly UsersService _usersService;

        public LearningQuizzesController(LearningQuizzesService learningQuizzesService, UsersService usersService) {
            _learningQuizzesService = learningQuizzesService;
            _usersService = usersService;
        }


        // GETOS

        [AllowAnonymous]
        [HttpGet("{quizId}/anonymous-learning-quiz-instances")]
        public async Task<IActionResult> GetAllLearningQuizInstancesOfQuiz(Guid quizId) {
            if (UserId != null)
                return BadRequest("You are logged in");

            var anonymousUserId = await _usersService.GetAnonymousUserId(Request);
            if (anonymousUserId == null)
                return Ok(new long[] { });

            var instances = await _learningQuizzesService.GetAllLearningQuizInstancesOfQuizAsync(quizId, anonymousUserId);

            return Ok(instances);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLearningQuizzesAsync() {
            var learningQuizzes = await _learningQuizzesService.GetAllLearningQuizzes(null, UserId);
            if (learningQuizzes == null)
                return NotFound();
            return Ok(learningQuizzes);
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetAllCurrentLearningQuizzesAsync() {
            var learningQuizzes = await _learningQuizzesService.GetAllLearningQuizzes(false, UserId);
            if (learningQuizzes == null)
                return NotFound();
            return Ok(learningQuizzes);
        }

        [HttpGet("finished")]
        public async Task<IActionResult> GetAllFinishedLearningQuizzesAsync() {
            var learningQuizzes = await _learningQuizzesService.GetAllLearningQuizzes(true, UserId);
            if (learningQuizzes == null)
                return NotFound();
            return Ok(learningQuizzes);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLearningQuizByIdAsync(long id) {
            var userId = UserId ?? await _usersService.GetAnonymousUserId(Request);
            var learningQuiz = await _learningQuizzesService.GetLearningQuizByIdAsync(id, userId);
            if (learningQuiz == null)
                return NotFound();
            return Ok(learningQuiz);
        }

        [AllowAnonymous]
        [HttpGet("{id}/next-question")]
        public async Task<IActionResult> GetNextQuestionAsync(long id) {
            var userId = UserId ?? await _usersService.GetAnonymousUserId(Request);
            var nextQuestionResult = await _learningQuizzesService.NextQuestionAsync(id, userId);
            if (nextQuestionResult == null)
                return NotFound();
            return Ok(nextQuestionResult);
        }


        // POSTOS
        
        [AllowAnonymous]
        [HttpPost("{quizId}")]
        public async Task<IActionResult> CreateLearningQuiz(Guid quizId) {
            var userId = UserId ?? await _usersService.GetAnonymousUserId(Request) ?? await _usersService.GenerateAnonymousUserId(Response);
            var learningQuiz = await _learningQuizzesService.CreateLearningQuizAsync(quizId, userId);
            if (learningQuiz == null)
                return BadRequest();
            return Ok(learningQuiz);
        }

        [AllowAnonymous]
        [HttpPost("{learningQuizId}/answer")]
        public async Task<IActionResult> AnswerTheQuestion(long learningQuizId, LearningQuizUserAnswer userAnswer) {
            var userId = UserId ?? await _usersService.GetAnonymousUserId(Request);
            var result = await _learningQuizzesService.AnswerQuestion(learningQuizId, userAnswer, userId);
            if (result == null)
                return BadRequest();
            return Ok(result);
        }


        // DELETOS

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuizAsync(long id) {
            var result = await _learningQuizzesService.DeleteLearningQuizAsync(id, UserId);
            if (!result)
                return NotFound();
            return Ok();
        }
    }
}