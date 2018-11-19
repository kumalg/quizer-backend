using System;
using Microsoft.AspNetCore.Mvc;
using quizer_backend.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Services;

namespace quizer_backend.Controllers {

    [Route("quizes")]
    public class QuizzesController : QuizerApiControllerBase {

        private readonly QuizzesService _quizzesService;
        private readonly StatisticsService _statisticsService;
        private readonly SolvingQuizzesService _solvingQuizzesService;

        public QuizzesController(QuizzesService quizzesService, StatisticsService statisticsService, SolvingQuizzesService solvingQuizzesService) {
            _quizzesService = quizzesService;
            _statisticsService = statisticsService;
            _solvingQuizzesService = solvingQuizzesService;
        }


        // GETOS

        [HttpGet]
        public async Task<IActionResult> GetAllQuizzes() {
            var quizzes = await _quizzesService.GetUserQuizzesAsync(UserId);
            return Ok(quizzes);
        }

        [HttpGet("{quizId}/attached-to-me")]
        public async Task<IActionResult> IsQuizAttachedToUser(Guid quizId) {
            var attached = await _quizzesService.IsQuizAttachedToUserAsync(quizId, UserId);
            return Ok(attached);
        }

        [HttpGet("{quizId}")]
        public async Task<IActionResult> GetQuizByIdAsync(Guid quizId) {
            var quiz = await _quizzesService.GetByIdAsync(quizId, UserId);
            if (quiz == null)
                return NotFound();
            return Ok(quiz);
        }

        [HttpGet("{quizId}/stats")]
        public async Task<IActionResult> GetQuizStatisticsByIdAsync(Guid quizId) {
            var stats = await _statisticsService.GetQuizStatistics(quizId, UserId);
            if (stats == null)
                return NotFound();
            return Ok(stats);
        }

        [HttpGet("{quizId}/questions")]
        public async Task<IActionResult> GetQuestionsByQuizId(Guid quizId, long? maxTime = null) {
            var questions = await _quizzesService.QuestionsInQuizAsync(quizId, UserId, maxTime);
            if (questions == null)
                return NotFound();
            return Ok(questions);
        }

        [AllowAnonymous]
        [HttpGet("{id}/solve")]
        public async Task<IActionResult> GetRandomQuestionsWithQuizByQuizIdAsync(Guid id) {
            var result = await _solvingQuizzesService.GetRandomQuestionsWithQuizByQuizIdAsync(id, UserId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }


        // POSTOS

        [HttpPost]
        public async Task<IActionResult> CreateQuizAsync(Quiz quiz) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var result = await _quizzesService.CreateQuizAsync(quiz, UserId);
            if (!result)
                return BadRequest();
            return Ok(quiz);
        }

        [HttpPost("{quizId}/give-creator-access")]
        public async Task<IActionResult> GiveCreatorAccess(Guid quizId, string email) {
            var result = await _quizzesService.GiveAccess(quizId, UserId, email, QuizAccessEnum.Creator);
            if (!result)
                return NotFound();
            return Ok();
        }

        [HttpPost("{quizId}/give-solver-access")]
        public async Task<IActionResult> GiveSolverAccess(Guid quizId, string email) {
            var result = await _quizzesService.GiveAccess(quizId, UserId, email, QuizAccessEnum.Solver);
            if (!result)
                return NotFound();
            return Ok();
        }

        [HttpPost("{quizId}/stop-co-creating")]
        public async Task<IActionResult> StopCoCreating(Guid quizId) {
            var result = await _quizzesService.StopCreating(quizId, UserId);
            if (!result)
                return NotFound();
            return Ok();
        }

        [HttpPost("{quizId}/leave")]
        public async Task<IActionResult> Leave(Guid quizId) {
            var result = await _quizzesService.RemoveAccess(quizId, UserId);
            if (!result)
                return BadRequest();
            return Ok();
        }

        [HttpPost("{quizId}/join")]
        public async Task<IActionResult> Join(Guid quizId) {
            var result = await _quizzesService.AddAccess(quizId, UserId);
            if (!result)
                return BadRequest();
            return Ok();
        }


        // PUTOS

        [HttpPut("{quizId}")]
        public async Task<IActionResult> UpdateQuizAsync(Guid quizId, Quiz newQuiz) {
            var quiz = await _quizzesService.UpdateQuizAsync(newQuiz, UserId);
            if (quiz == null)
                return BadRequest();
            return Ok(quiz);
        }


        // DELETOS

        [HttpDelete("{quizId}")]
        public async Task<IActionResult> DeleteQuizAsync(Guid quizId) {
            var deleted = await _quizzesService.DeleteQuiz(quizId, UserId);
            if (!deleted)
                return BadRequest();
            return Ok();
        }
    }
}