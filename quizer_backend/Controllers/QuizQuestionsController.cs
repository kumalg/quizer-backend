using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quizer_backend.Data;
using quizer_backend.Data.Entities;
using System.Linq;
using System.Security.Claims;

namespace quizer_backend.Controllers {
    [Authorize]
    [ApiController]
    [Route("quiz-questions")]
    [Produces("application/json")]
    public class QuizQuestionsController : ControllerBase {
        private readonly IQuizerRepository _repository;

        public QuizQuestionsController(IQuizerRepository repository) {
            _repository = repository;
        }

        private string UserId(ClaimsPrincipal User) => User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

        [HttpPost]
        public ActionResult<QuizQuestionItem> CreateQuizQuestion(QuizQuestionItem question) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            _repository.AddQuizQuestion(question);
            return question;
        }
    }
}