using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using quizer_backend.Data.Entities.SolvedQuiz;
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
        public async Task<SolvedQuiz> GetSolvedQuiz(long id) {
            return await _statisticsService.GetSolvedQuizById(id);
        }


        // PUTOS

        //[HttpPut]
        //public async Task<IActionResult> UpdateUserSettings(NewUserSettings userSettings) {
        //    var settings = await _usersService.UpdateUserSettingsAsync(userSettings, UserId);
        //    if (settings == null)
        //        return BadRequest();
        //    return Ok(settings);
        //}
    }
}
