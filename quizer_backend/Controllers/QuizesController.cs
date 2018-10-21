using Microsoft.AspNetCore.Mvc;
using quizer_backend.Data;
using quizer_backend.Data.Entities;
using quizer_backend.Models;
using System.Threading.Tasks;

namespace quizer_backend.Controllers {

    [Route("quizes")]
    public class QuizesController : QuizerApiControllerBase {

        public QuizesController(IQuizerRepository repository) : base(repository) { }


        // GETOS

        [HttpGet]
        public ActionResult GetAllQuizes() {
            var quizes = _repository.GetAllQuizes(UserId(User));
            return ToJsonContentResult(quizes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetQuizByIdAsync(long id) {
            var quiz = await _repository.GetQuizByIdAsync(UserId(User), id);
            return ToJsonContentResult(quiz);
        }

        [HttpGet("{id}/questions")]
        public async Task<ActionResult> GetQuizQuestionsByQuizId(long id) {
            var quiz = await _repository.GetQuizQuestionsByQuizIdAsync(UserId(User), id);
            return ToJsonContentResult(quiz);
        }


        // POSTOS

        [HttpPost]
        public async Task<ActionResult> CreateQuizAsync(QuizItem quiz) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var userId = UserId(User);
            quiz.OwnerId = userId;
            await _repository.AddQuizAsync(quiz);
            await _repository.AddQuizAccessAsync(new QuizAccess { Access = QuizAccessEnum.Owner, QuizId = quiz.Id, UserId = userId });
            return ToJsonContentResult(quiz);
        }


        // PUTOS

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateQuizNameAsync(long id, string name) {
            var quiz = await _repository.GetQuizByIdAsync(UserId(User), id);

            if (quiz == null)
                return NotFound();
            if (string.IsNullOrEmpty(name))
                return BadRequest();

            quiz.Name = name;
            await _repository.SaveAllAsync();

            return ToJsonContentResult(quiz);
        }


        // DELETOS

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteQuizAsync(long id) {
            bool deleted = await _repository.DeleteQuizByIdAsync(UserId(User), id);

            if (deleted)
                return Ok();

            return BadRequest();
        }
    }
}