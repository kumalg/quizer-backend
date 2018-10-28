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
        public async Task<ActionResult> CreateQuizQuestionAnswerAsync(QuizQuestionAnswer answer) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var result = await _repository.AddQuizQuestionAnswerWithVersionAsync(answer);
            if (!result)
                return BadRequest();

            return ToJsonContentResult(new {
                answer.Id,
                answer.QuizQuestionId,
                answer.Value,
                answer.IsCorrect
            });
        }


        // PUTOS

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateQuizQuestionAnswer(long id, QuizQuestionAnswer newAnswer) {
            QuizQuestionAnswer answer = await _repository.GetQuizQuestionAnswerByIdAsync(UserId(User), id);

            if (string.IsNullOrEmpty(newAnswer.Value))
                return BadRequest();
            if (answer == null)
                return NotFound();

            var answerVersion = new QuizQuestionAnswerVersion {
                QuizQuestionAnswerId = newAnswer.Id,
                Value = newAnswer.Value,
                IsCorrect = newAnswer.IsCorrect
            };

            var result = await _repository.AddQuizQuestionAnswerVersionAsync(answerVersion);
            if (!result)
                return BadRequest();

            answer.IsCorrect = newAnswer.IsCorrect;
            answer.Value = newAnswer.Value;
            
            return ToJsonContentResult(new {
                answer.Id,
                answer.QuizQuestionId,
                answer.Value,
                answer.IsCorrect
            });
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