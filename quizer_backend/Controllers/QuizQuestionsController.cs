using Microsoft.AspNetCore.Mvc;
using quizer_backend.Data;
using quizer_backend.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace quizer_backend.Controllers {

    [Route("quiz-questions")]
    public class QuizQuestionsController : QuizerApiControllerBase {

        public QuizQuestionsController(IQuizerRepository repository) : base(repository) { }


        // GETOS

        [HttpGet("{id}/answers")]
        public async Task<ActionResult<List<QuizQuestionAnswer>>> GetQuizQuestionAnswers(long id, long? maxTime = null) {
            var answers = await _repository.GetQuizQuestionAnswersByQuizQuestionIdAsync(UserId(User), id, maxTime: maxTime);

            if (answers == null)
                NotFound();

            foreach (var answer in answers)
                answer.FlatVersionProps(maxTime);

            return ToJsonContentResult(answers.Select(a => new {
                a.Id,
                a.QuizQuestionId,
                a.Value,
                a.IsCorrect
            }));
        }


        // POSTOS

        [HttpPost]
        public async Task<ActionResult> CreateQuizQuestionAsync(QuizQuestion question) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var result = await _repository.AddQuizQuestionWithVersionAsync(question);
            if (!result)
                return BadRequest();

            return ToJsonContentResult(new {
                question.Id,
                question.QuizId,
                question.Value
            });
        }


        // PUTOS

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateQuizQuestionValue(long id, string value) {
            QuizQuestion question = await _repository.GetQuizQuestionByIdAsync(UserId(User), id);

            if (string.IsNullOrEmpty(value))
                return BadRequest();
            if (question == null)
                return NotFound();

            var questionVersion = new QuizQuestionVersion {
                QuizQuestionId = question.Id,
                Value = value
            };

            var result = await _repository.AddQuizQuestionVersionAsync(questionVersion);
            if (!result)
                return BadRequest();

            question.Value = value;
            return ToJsonContentResult(new {
                question.Id,
                question.QuizId,
                question.Value
            });
        }


        // DELETOS

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteQuizQuestion(long id) {
            bool deleted = await _repository.DeleteQuizQuestionByIdAsync(UserId(User), id);
            if (deleted) return Ok();
            return BadRequest();
        }
    }
}