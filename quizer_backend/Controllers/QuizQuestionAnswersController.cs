using Microsoft.AspNetCore.Mvc;
using quizer_backend.Data;
using quizer_backend.Data.Entities;
using System.Threading.Tasks;

namespace quizer_backend.Controllers {

    [Route("quiz-question-answers")]
    public class QuizQuestionAnswersController : QuizerApiControllerBase {

        public QuizQuestionAnswersController(IQuizerRepository repository) : base(repository) { }


        // POSTOS

        [HttpPost]
        public async Task<ActionResult> CreateQuizQuestionAnswerAsync(QuizQuestionAnswerItem answer) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            await _repository.AddQuizQuestionAnswerAsync(answer);
            return ToJsonContentResult(answer);
        }


        // PUTOS

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateQuizQuestionAnswer(long id, QuizQuestionAnswerItem newAnswer) {
            QuizQuestionAnswerItem answer = await _repository.GetQuizQuestionAnswerByIdAsync(UserId(User), id);
            if (answer == null) return NotFound();
            if (string.IsNullOrEmpty(answer.Value)) return BadRequest();
            answer.IsCorrect = newAnswer.IsCorrect;
            answer.Value = newAnswer.Value;
            await _repository.SaveAllAsync();
            return ToJsonContentResult(answer);
        }


        // DELETOS

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteQuizQuestionAnswer(long id) {
            bool deleted = await _repository.DeleteQuizQuestionAnswerByIdAsync(UserId(User), id);
            if (deleted) return Ok();
            return BadRequest();
        }
    }
}