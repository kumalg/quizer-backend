using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Entities.QuizObjectVersion;
using quizer_backend.Data.SuperRepository;

namespace quizer_backend.Controllers {

    [Route("quiz-questions")]
    public class QuestionsController : QuizerApiControllerBase {
        
        private readonly QuizzesRepository _quizzesRepository;
        private readonly AnswersRepository _answersRepository;
        private readonly QuestionsRepository _questionsRepository;
        private readonly QuestionVersionsRepository _questionVersionsRepository;

        public QuestionsController(
            QuizzesRepository quizzesRepository,
            QuestionsRepository questionsRepository,
            QuestionVersionsRepository questionVersionsRepository,
            AnswersRepository answersRepository,
            QuizAccessesRepository quizAccessesRepository
        ) {
            _quizzesRepository = quizzesRepository;
            _questionsRepository = questionsRepository;
            _questionVersionsRepository = questionVersionsRepository;
            _answersRepository = answersRepository;
        }


        // GETOS

        [HttpGet("{questionId}/answers")]
        public async Task<IActionResult> GetQuizQuestionAnswers(long questionId, long? maxVersionTime = null) {
            var question = await _questionsRepository.GetById(questionId);

            var access = await _quizzesRepository.HaveReadAccessToQuizAsync(UserId, question.QuizId);
            if (!access)
                return NotFound();

            var answers = _answersRepository
                .GetAllByQuestionId(questionId, maxVersionTime)
                .Include(a => a.Versions)
                .Select(a => a.FlatVersionProps(maxVersionTime));

            return Ok(answers);
        }


        // POSTOS

        [HttpPost]
        public async Task<IActionResult> CreateQuizQuestionAsync(Question question) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var access = await _quizzesRepository.HaveWriteAccessToQuiz(UserId, question.QuizId);
            if (!access)
                return BadRequest();

            var creationTime = CurrentTime;
            question.CreationTime = creationTime;
            await _questionsRepository.Create(question);
            
            var questionVersion = new QuestionVersion {
                CreationTime = creationTime,
                QuizQuestionId = question.Id,
                Value = question.Value
            };
            await _questionVersionsRepository.Create(questionVersion);

            return Created("quiz-questions", question);
        }


        // PUTOS

        [HttpPut("{questionId}")]
        public async Task<IActionResult> UpdateQuestion(long questionId, string value) {
            if (string.IsNullOrEmpty(value))
                return BadRequest("value cannot be empty");

            var question = await _questionsRepository
                .GetAll()
                .Where(a => a.Id == questionId)
                .Where(a => !a.IsDeleted)
                .SingleOrDefaultAsync();

            if (question == null)
                return NotFound();

            var access = await _quizzesRepository.HaveWriteAccessToQuiz(UserId, question.QuizId);
            if (!access)
                return NotFound();
            
            var questionVersion = new QuestionVersion {
                CreationTime = CurrentTime,
                QuizQuestionId = questionId,
                Value = value
            };
            await _questionVersionsRepository.Create(questionVersion);

            question.Value = value;
            return Ok(question);
        }


        // DELETOS

        [HttpDelete("{questionId}")]
        public async Task<IActionResult> DeleteQuestion(long questionId) {
            var question = await _questionsRepository.GetById(questionId);

            var access = await _quizzesRepository.HaveWriteAccessToQuiz(UserId, question.QuizId);
            if (!access)
                return NotFound();
            
            var deleted = await _questionsRepository.SilentDelete(questionId);
            if (!deleted)
                return NotFound();

            return Ok();
        }
    }
}