using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Services;

namespace quizer_backend.Controllers {

    [Route("quiz-question-answers")]
    public class AnswersController : QuizerApiControllerBase {

        private readonly AnswersService _answersService;

        public AnswersController(AnswersService answersService) {
            _answersService = answersService;
        }


        // POSTOS

        [HttpPost]
        public async Task<IActionResult> CreateAnswerAsync(Answer newAnswer) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var answer = await _answersService.CreateAnswerAsync(newAnswer, UserId);
            if (answer == null)
                return BadRequest();
            return Ok(answer);
        }


        // PUTOS

        [HttpPut("{answerId}")]
        public async Task<IActionResult> UpdateAnswerAsync(long answerId, Answer newAnswer) {
            var answer = await _answersService.UpdateAnswerAsync(newAnswer, UserId);
            if (answer == null)
                return BadRequest();
            return Ok(answer);
        }


        // DELETOS

        [HttpDelete("{answerId}")]
        public async Task<IActionResult> DeleteAnswer(long answerId) {
            var result = await _answersService.DeleteAnswerAsync(answerId, UserId);
            if (!result)
                return NotFound();
            return Ok();
        }
    }
}