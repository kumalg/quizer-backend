using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Entities.QuizObjectVersion;
using quizer_backend.Data.Repository;

namespace quizer_backend.Controllers {

    [Route("quiz-question-answers")]
    public class AnswersController : QuizerApiControllerBase {

        private readonly QuizzesRepository _quizzesRepository;
        private readonly AnswersRepository _answersRepository;
        private readonly AnswerVersionsRepository _answerVersionsRepository;
        private readonly QuestionsRepository _questionsRepository;

        public AnswersController(
            QuizzesRepository quizzesRepository,
            QuestionsRepository questionsRepository,
            QuestionVersionsRepository questionVersionsRepository,
            AnswersRepository answersRepository,
            AnswerVersionsRepository answerVersionsRepository,
            QuizAccessesRepository quizAccessesRepository
        ) {
            _quizzesRepository = quizzesRepository;
            _questionsRepository = questionsRepository;
            _answersRepository = answersRepository;
            _answerVersionsRepository = answerVersionsRepository;
        }


        // POSTOS

        [HttpPost]
        public async Task<IActionResult> CreateAnswerAsync(Answer answer) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var question = await _questionsRepository.GetById(answer.QuestionId);
            if (question == null)
                return BadRequest();

            var access = await _quizzesRepository.HaveWriteAccessToQuiz(UserId, question.QuizId);
            if (!access)
                return BadRequest();

            var creationTime = CurrentTime;
            answer.CreationTime = creationTime;
            await _answersRepository.Create(answer);

            var answerVersion = new AnswerVersion {
                CreationTime = creationTime,
                QuizQuestionAnswerId = answer.Id,
                Value = answer.Value
            };
            await _answerVersionsRepository.Create(answerVersion);

            return Created("quiz-question-answers", answer);
        }


        // PUTOS

        [HttpPut("{answerId}")]
        public async Task<IActionResult> UpdateAnswerAsync(long answerId, Answer newAnswer) {
            if (string.IsNullOrEmpty(newAnswer.Value))
                return BadRequest();

            var answer = await _answersRepository
                .GetAll()
                .Where(a => a.Id == answerId)
                .Where(a => !a.IsDeleted)
                .Include(a => a.Question)
                .SingleOrDefaultAsync();

            if (answer == null)
                return NotFound();

            var access = await _quizzesRepository.HaveWriteAccessToQuiz(UserId, answer.Question.QuizId);
            if (!access)
                return BadRequest();

            var answerVersion = new AnswerVersion {
                CreationTime = CurrentTime,
                QuizQuestionAnswerId = newAnswer.Id,
                Value = newAnswer.Value,
                IsCorrect = newAnswer.IsCorrect
            };

            await _answerVersionsRepository.Create(answerVersion);

            answer.IsCorrect = newAnswer.IsCorrect;
            answer.Value = newAnswer.Value;
            
            return Ok(answer);
        }


        // DELETOS

        [HttpDelete("{answerId}")]
        public async Task<IActionResult> DeleteAnswer(long answerId) {
            var quizId = await _answersRepository.GetQuizId(answerId);

            var access = await _quizzesRepository.HaveWriteAccessToQuiz(UserId, quizId);
            if (!access)
                return NotFound();

            var deleted = await _answersRepository.SilentDelete(answerId);
            if (!deleted)
                return NotFound();

            return Ok();
        }
    }
}