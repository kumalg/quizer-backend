using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Services;

namespace quizer_backend.Controllers {

    [Route("quiz-questions")]
    public class QuestionsController : QuizerApiControllerBase {

        private readonly QuestionsService _questionsService;

        public QuestionsController(QuestionsService questionsService) {
            _questionsService = questionsService;
        }


        // GETOS

        [HttpGet("{questionId}/answers")]
        public async Task<IActionResult> GetQuizQuestionAnswers(long questionId, long? maxVersionTime = null) {
            var answers = await _questionsService.GetQuestionAnswersAsync(questionId, UserId, maxVersionTime);
            if (answers == null)
                return NotFound();
            return Ok(answers);
        }


        // POSTOS

        [HttpPost]
        public async Task<IActionResult> CreateQuizQuestionAsync(Question newQuestion) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var question = await _questionsService.CreateQuestionAsync(newQuestion, UserId);
            if (question == null)
                return BadRequest();
            return Ok(question);
        }


        // PUTOS

        [HttpPut("{questionId}")]
        public async Task<IActionResult> UpdateQuestion(long questionId, string value) {
            if (string.IsNullOrEmpty(value))
                return BadRequest("value cannot be empty");

            var question = await _questionsService.UpdateQuestionAsync(questionId, value, UserId);
            if (question == null)
                return BadRequest();
            return Ok(question);
        }


        // DELETOS

        [HttpDelete("{questionId}")]
        public async Task<IActionResult> DeleteQuestion(long questionId) {
            var deleted = await _questionsService.DeleteQuestionAsync(questionId, UserId);
            if (!deleted)
                return NotFound();
            return Ok();
        }
    }
}