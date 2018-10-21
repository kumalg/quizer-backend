using Microsoft.AspNetCore.Mvc;
using quizer_backend.Data;
using quizer_backend.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace quizer_backend.Controllers {

    [Route("quiz-questions")]
    public class QuizQuestionsController : QuizerApiControllerBase {

        public QuizQuestionsController(IQuizerRepository repository) : base(repository) { }


        // GETOS

        [HttpGet("{id}/answers")]
        public async Task<ActionResult<List<QuizQuestionAnswerItem>>> GetQuizQuestionAnswers(long id) {
            var answers = await _repository.GetQuizQuestionAnswersByQuizQuestionIdAsync(UserId(User), id);
            if (answers == null) NotFound();
            return ToJsonContentResult(answers);
        }


        // POSTOS

        [HttpPost]
        public async Task<ActionResult> CreateQuizQuestionAsync(QuizQuestionItem question) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            await _repository.AddQuizQuestionAsync(question);
            return ToJsonContentResult(question);
        }


        // PUTOS

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateQuizQuestionValue(long id, string value) {
            QuizQuestionItem question = await _repository.GetQuizQuestionByIdAsync(UserId(User), id);
            if (question == null) return NotFound();
            if (string.IsNullOrEmpty(value)) return BadRequest();
            question.Value = value;
            await _repository.SaveAllAsync();
            return ToJsonContentResult(question);
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