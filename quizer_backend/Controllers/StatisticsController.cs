using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using quizer_backend.Data.Services;

namespace quizer_backend.Controllers {

    [Route("statistics")]
    public class StatisticsController : QuizerApiControllerBase {

        private readonly StatisticsService _statisticsService;

        public StatisticsController(StatisticsService statisticsService) {
            _statisticsService = statisticsService;
        }


        // GETOS

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSolvedQuiz(long id) {
            var result = await _statisticsService.GetSolvedQuizById(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserStatistics() {
            var result = await _statisticsService.GetUserStatistics(UserId);
            return Ok(result);
        }

        [HttpGet("quiz-list")]
        public async Task<IActionResult> GetQuizListForStatistics() {
            var list = await _statisticsService.GetQuizListForStatistics(UserId);
            return Ok(list);
        }
    }
}
