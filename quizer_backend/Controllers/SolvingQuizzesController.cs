using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quizer_backend.Data.Services;
using quizer_backend.Models;

namespace quizer_backend.Controllers {

    [Route("solving-quizzes")]
    public class SolvingQuizzesController : QuizerApiControllerBase {

        private readonly SolvingQuizzesService _solvingQuizzesService;
        private readonly StatisticsService _statisticsService;

        public SolvingQuizzesController(SolvingQuizzesService solvingQuizzesService, StatisticsService statisticsService) {
            _solvingQuizzesService = solvingQuizzesService;
            _statisticsService = statisticsService;
        }


        // POSTOS

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PostSolvedQuiz(UserSolvedQuiz solvedQuiz) {
            var result = await _solvingQuizzesService.CheckSolvedQuizAsync(solvedQuiz, UserId);
            if (result == null)
                return BadRequest();
            await _statisticsService.IncreaseSolveSessions(solvedQuiz.QuizId);
            return Ok(result);
        }
    }
}