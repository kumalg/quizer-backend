using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using quizer_backend.Data;
using quizer_backend.Data.Entities;

namespace quizer_backend.Controllers {
    [Produces("application/json")]
    [Route("quizes")]
    [EnableCors("CorsPolicy")]
    [ApiController]
    [Authorize]
    public class QuizesController : ControllerBase {
        private readonly IQuizerRepository _repository;

        public QuizesController(IQuizerRepository repository) {
            _repository = repository;
        }

        private string UserId(ClaimsPrincipal User) => User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

        private ContentResult ToJsonContentResult(object item) {
            JsonSerializerSettings settings = new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };
            var json = JsonConvert.SerializeObject(item, settings);
            return Content(json, "application/json");
        }

        [HttpGet]
        public ActionResult GetAll() {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var quizes = _repository.GetAllMyQuizes(userId);
            return ToJsonContentResult(quizes);
        }

        [HttpGet("my")]
        public ActionResult GetAllMyQuizes() {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var quizes = _repository.GetAllMyQuizes(userId);
            return ToJsonContentResult(quizes);
        }

        [HttpPost]
        public ActionResult<QuizItem> CreateQuiz(QuizItem quiz) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            quiz.OwnerId = UserId(User);
            _repository.AddQuiz(quiz);
            return quiz;
        }

        [HttpPut("{id}")]
        public ActionResult<QuizItem> UpdateQuizName(long id, string name) {
            var quiz = _repository.GetQuizById(id);
            if (quiz == null || string.IsNullOrEmpty(name))
                return BadRequest();
            quiz.Name = name;
            _repository.SaveAll();
            return quiz;
        }

        [HttpPost("{id}/questions")]
        public ActionResult<QuizQuestionItem> CreateQuizQuestion(long id, QuizQuestionItem question) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            _repository.AddQuizQuestion(question);
            return question;
        }

        //[HttpGet("{id}", Name = "GetQuiz")]
        //public ActionResult<QuizItem> GetById(long id) {
        //    var item = _context.QuizItems.Find(id);
        //    if (item == null) {
        //        return NotFound();
        //    }
        //    return item;
        //}
    }
}