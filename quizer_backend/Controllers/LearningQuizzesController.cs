using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities.LearningQuiz;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Repository;
using quizer_backend.Helpers;
using quizer_backend.Models;
using quizer_backend.Services;

namespace quizer_backend.Controllers {

    [Route("learning-quizes")]
    public class LearningQuizzesController : QuizerApiControllerBase {

        private readonly Auth0ManagementFactory _auth0ManagementFactory;
        private readonly UserSettingsRepository _userSettingsRepository;
        private readonly QuizzesRepository _quizzesRepository;
        private readonly QuizAccessesRepository _quizAccessesRepository;
        private readonly QuestionsRepository _questionsRepository;
        private readonly AnswersRepository _answersRepository;
        private readonly LearningQuizzesRepository _learningQuizzesRepository;
        private readonly LearningQuizQuestionsRepository _learningQuizQuestionsRepository;

        public LearningQuizzesController(
            QuizzesRepository quizzesRepository,
            UserSettingsRepository userSettingsRepository,
            QuizAccessesRepository quizAccessesRepository,
            QuestionsRepository questionsRepository,
            AnswersRepository answersRepository,
            LearningQuizzesRepository learningQuizzesRepository,
            LearningQuizQuestionsRepository learningQuizQuestionsRepository,
            Auth0ManagementFactory auth0ManagementFactory
        ) {
            _auth0ManagementFactory = auth0ManagementFactory;
            _userSettingsRepository = userSettingsRepository;
            _quizzesRepository = quizzesRepository;
            _quizAccessesRepository = quizAccessesRepository;
            _questionsRepository = questionsRepository;
            _answersRepository = answersRepository;
            _learningQuizzesRepository = learningQuizzesRepository;
            _learningQuizQuestionsRepository = learningQuizQuestionsRepository;
        }

        private async Task<ManagementApiClient> GetManagementApiClientAsync()
            => await _auth0ManagementFactory.GetManagementApiClientAsync();


        // GETOS

        [HttpGet]
        public async Task<IActionResult> GetAllLearningQuizzesAsync() {
            var learningQuizzes = await _learningQuizzesRepository
                .GetAllByUserId(UserId)
                .Include(q => q.Quiz)
                .ToListAsync();

            foreach (var learningQuiz in learningQuizzes) {
                if (learningQuiz.QuizId == null) continue;
                var access = await _quizAccessesRepository.GetQuizAccessForUserAsync(UserId, learningQuiz.QuizId.Value);
                learningQuiz.Quiz.IncludeAccess(access.Access);
            }

            var learningQuizzesWithOwners = await IncludeOwnerNickNames(learningQuizzes);
            return Ok(learningQuizzesWithOwners);
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetAllCurrentLearningQuizzesAsync() {
            var learningQuizzes = await _learningQuizzesRepository
                .GetAllByUserId(UserId)
                .Where(q => !q.IsFinished)
                .Include(q => q.Quiz)
                .ToListAsync();

            for (var i = learningQuizzes.Count - 1; i >= 0; i--) {
                if (learningQuizzes[i].QuizId == null)
                    continue;

                var access = await _quizAccessesRepository.GetQuizAccessForUserAsync(UserId, learningQuizzes[i].QuizId.Value);
                if (access == null || access.Access == QuizAccessEnum.None) {
                    learningQuizzes.RemoveAt(i);
                }
                else {
                    learningQuizzes[i].Quiz.IncludeAccess(access.Access);
                }
            }

            var learningQuizzesWithOwners = await IncludeOwnerNickNames(learningQuizzes);
            return Ok(learningQuizzesWithOwners);
        }

        [HttpGet("finished")]
        public async Task<IActionResult> GetAllFinishedLearningQuizzesAsync() {
            var learningQuizzes = await _learningQuizzesRepository
                .GetAllByUserId(UserId)
                .Where(q => q.IsFinished)
                .Include(q => q.Quiz)
                .ToListAsync();

            for (var i = learningQuizzes.Count - 1; i >= 0; i--) {
                if (learningQuizzes[i].QuizId == null)
                    continue;

                var access = await _quizAccessesRepository.GetQuizAccessForUserAsync(UserId, learningQuizzes[i].QuizId.Value);
                if (access == null || access.Access == QuizAccessEnum.None) {
                    learningQuizzes.RemoveAt(i);
                }
                else {
                    learningQuizzes[i].Quiz.IncludeAccess(access.Access);
                }
            }

            var learningQuizzesWithOwners = await IncludeOwnerNickNames(learningQuizzes);
            return Ok(learningQuizzesWithOwners);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLearningQuizByIdAsync(long id) {
            var learningQuiz = await _learningQuizzesRepository
                .GetAllByUserId(UserId)
                .Where(q => q.Id == id)
                .Include(q => q.Quiz)
                .SingleOrDefaultAsync();

            if (learningQuiz?.QuizId == null)
                return NotFound();
            
            var access = await _quizzesRepository.HaveReadAccessToQuizAsync(UserId, learningQuiz.QuizId.Value);
            if (!access)
                return Forbid();

            await QuizItemWithOwnerNickName(learningQuiz.Quiz);
            return Ok(learningQuiz);
        }

        [HttpGet("{id}/next-question")]
        public async Task<IActionResult> GetNextQuestionAsync(long id) {
            var learningQuiz = await _learningQuizzesRepository.GetById(id);
            if (learningQuiz?.QuizId == null)
                return BadRequest();

            var access = await _quizzesRepository.HaveReadAccessToQuizAsync(UserId, learningQuiz.QuizId.Value);
            if (!access)
                return Forbid();

            var maxVersionTime = learningQuiz.CreationTime;
            var reoccurrences = _learningQuizQuestionsRepository.GetAllByLearningQuizId(learningQuiz.Id);

            if (reoccurrences == null)
                return BadRequest();

            var remainingReoccurrences = reoccurrences.Where(q => q.Reoccurrences > 0);

            if (!remainingReoccurrences.Any()) {
                return Ok(new { IsFinished = true });
            }

            var randomQuestionReoccurrences = remainingReoccurrences.RandomElement(p => true);
            
            var randomQuestion = await _questionsRepository
                .GetAll()
                .Where(q => q.Id == randomQuestionReoccurrences.QuestionId)
                .Where(q => q.CreationTime <= maxVersionTime)
                .Include(q => q.Versions)
                .SingleOrDefaultAsync();

            randomQuestion.FlatVersionProps(maxVersionTime);
            
            var answers = await _answersRepository
                .GetAllByQuestionId(randomQuestion.Id, maxVersionTime, true)
                .Include(a => a.Versions)
                .ToListAsync();

            foreach (var answer in answers)
                answer.FlatVersionProps(maxVersionTime);

            return Ok(new {
                IsFinished = false,
                randomQuestionReoccurrences.Reoccurrences,
                Question = randomQuestion,
                Answers = answers.Select(a => new {
                    a.Id,
                    a.Value
                })
            });
        }


        // POSTOS

        [HttpPost("{quizId}")]
        public async Task<IActionResult> CreateLearningQuiz(long quizId) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var access = await _quizzesRepository.HaveReadAccessToQuizAsync(UserId, quizId);
            if (!access)
                return NotFound();

            var creationTime = CurrentTime;

            var questionsQuery = _questionsRepository
                .GetAllByQuizId(quizId)
                .Where(q => q.CreationTime <= creationTime);

            var learningQuiz = new LearningQuiz {
                CreationTime = creationTime,
                UserId = UserId,
                QuizId = quizId,
                NumberOfQuestions = await questionsQuery.CountAsync()
            };
            await _learningQuizzesRepository.Create(learningQuiz);

            var settings = await _userSettingsRepository.GetByIdOrDefault(UserId);
            var questions = questionsQuery
                .Select(q => new LearningQuizQuestion {
                    LearningQuizId = learningQuiz.Id,
                    QuestionId = q.Id,
                    Reoccurrences = settings.ReoccurrencesOnStart
                });
            await _learningQuizQuestionsRepository.CreateMany(questions);

            return Ok(learningQuiz);
        }

        [HttpPost("{learningQuizId}/answer")]
        public async Task<IActionResult> AnswerTheQuestion(long learningQuizId, LearningQuizUserAnswer userAnswer) {
            var learningQuiz = await _learningQuizzesRepository.GetById(learningQuizId);
            if (learningQuiz == null)
                return BadRequest();
            if (learningQuiz.UserId != UserId)
                return NotFound();

            var answers = await _answersRepository
                .GetAllByQuestionId(userAnswer.QuizQuestionId, learningQuiz.CreationTime, true)
                .Include(a => a.Versions)
                .ToListAsync();
            if (answers == null)
                return BadRequest();

            var questionReoccurrences = await _learningQuizQuestionsRepository
                .GetAllByLearningQuizId(learningQuizId)
                .Where(q => q.QuestionId == userAnswer.QuizQuestionId)
                .SingleOrDefaultAsync();

            if (questionReoccurrences == null)
                return BadRequest();

            var correctAnswers = answers
                .Select(i => i.FlatVersionProps(learningQuiz.CreationTime))
                .Where(i => i.IsCorrect)
                .Select(i => i.Id)
                .OrderBy(i => i)
                .ToList();
            var selectedAnswers = userAnswer.SelectedAnswers
                .OrderBy(i => i)
                .ToList();

            var settings = await _userSettingsRepository.GetByIdOrDefault(UserId);
            var isCorrect = correctAnswers.SequenceEqual(selectedAnswers);
            if (isCorrect) {
                learningQuiz.NumberOfCorrectAnswers = learningQuiz.NumberOfCorrectAnswers + 1;
                questionReoccurrences.Reoccurrences = questionReoccurrences.Reoccurrences - 1;
                if (questionReoccurrences.Reoccurrences == 0) {
                    learningQuiz.NumberOfLearnedQuestions = learningQuiz.NumberOfLearnedQuestions + 1;
                }
            }
            else {
                learningQuiz.NumberOfBadAnswers = learningQuiz.NumberOfBadAnswers + 1;
                questionReoccurrences.Reoccurrences = questionReoccurrences.Reoccurrences + settings.ReoccurrencesIfBad;
                if (questionReoccurrences.Reoccurrences > settings.MaxReoccurrences)
                    questionReoccurrences.Reoccurrences = settings.MaxReoccurrences;
            }

            await _learningQuizQuestionsRepository.Update(questionReoccurrences.Id, questionReoccurrences);
            await _learningQuizzesRepository.Update(learningQuiz.Id, learningQuiz);

            var isUnfinished = await _learningQuizQuestionsRepository
                .GetAllByLearningQuizId(learningQuizId)
                .Where(q => q.Reoccurrences > 0)
                .AnyAsync();

            if (!isUnfinished)
                await _learningQuizzesRepository.SetAsFinished(learningQuizId, CurrentTime);

            return Ok(new {
                IsFinished = !isUnfinished,
                IsCorrect = isCorrect,
                CorrectAnswers = correctAnswers,
                SelectedAnswers = selectedAnswers,
                questionReoccurrences.Reoccurrences,
                LearningQuiz = learningQuiz
            });
        }


        // DELETOS

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuizAsync(long id) {
            var haveOwnerAccess = await _learningQuizzesRepository
                .GetAllByUserId(UserId)
                .Where(l => l.Id == id)
                .AnyAsync();
            if (!haveOwnerAccess)
                return NotFound();

            var deleted = await _learningQuizzesRepository.Delete(id);
            if (!deleted)
                return BadRequest();

            return Ok();
        }


        // PRIVATE HELPEROS

        private async Task<IEnumerable<LearningQuiz>> IncludeOwnerNickNames(IList<LearningQuiz> quizes) {
            var userIds = quizes
                .Where(q => q.QuizId != null)
                .Select(q => q.Quiz.OwnerId)
                .Distinct()
                .ToList();

            if (!userIds.Any())
                return quizes;

            var search = new GetUsersRequest {
                SearchEngine = "v3",
                Query = $"user_id: ({string.Join(" OR ", userIds)})"
            };
            var client = await GetManagementApiClientAsync();
            var owners = await client.Users.GetAllAsync(search);

            var yco = from quiz in quizes
                where quiz.QuizId != null
                join owner in owners on quiz.Quiz.OwnerId equals owner.UserId into users
                from user in users.DefaultIfEmpty()
                select quiz.IncludeOwnerNickNameInQuiz(user.NickName);

            return yco.Concat(quizes.Where(q => q.QuizId == null));
        }

        private async Task<Quiz> QuizItemWithOwnerNickName(Quiz quiz) {
            var client = await _auth0ManagementFactory.GetManagementApiClientAsync();
            var owner = await client.Users.GetAsync(quiz.OwnerId);

            if (owner != null)
                quiz.OwnerNickName = owner.NickName;

            return quiz;
        }
    }
}