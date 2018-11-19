using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities.LearningQuiz;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Repository;
using quizer_backend.Helpers;
using quizer_backend.Models;
using quizer_backend.Services;

namespace quizer_backend.Data.Services {
    public class LearningQuizzesService : BaseService {

        private readonly QuizzesRepository _quizzesRepository;
        private readonly QuizAccessesRepository _quizAccessesRepository;
        private readonly QuestionsRepository _questionsRepository;
        private readonly AnswersRepository _answersRepository;
        private readonly LearningQuizzesRepository _learningQuizzesRepository;
        private readonly LearningQuizQuestionsRepository _learningQuizQuestionsRepository;
        private readonly UserSettingsRepository _userSettingsRepository;
        private readonly Auth0UsersService _auth0UsersService;

        public LearningQuizzesService(
            QuizerContext context,
            Auth0ManagementFactory auth0ManagementFactory
        ) : base(context) {
            _quizzesRepository = new QuizzesRepository(context);
            _quizAccessesRepository = new QuizAccessesRepository(context);
            _questionsRepository = new QuestionsRepository(context);
            _answersRepository = new AnswersRepository(context);
            _learningQuizzesRepository = new LearningQuizzesRepository(context);
            _learningQuizQuestionsRepository = new LearningQuizQuestionsRepository(context);
            _userSettingsRepository = new UserSettingsRepository(context);
            _auth0UsersService = new Auth0UsersService(auth0ManagementFactory);
        }

        public async Task<IEnumerable<LearningQuiz>> GetAllLearningQuizzes(bool? isFinished, string userId) {
            var learningQuizzesQuery = _learningQuizzesRepository
                .GetAllByUserId(userId);

            if (isFinished != null) {
                learningQuizzesQuery = learningQuizzesQuery
                    .Where(l => l.IsFinished == isFinished.Value);
            }
            var learningQuizzes = await learningQuizzesQuery
                .Include(q => q.Quiz)
                .ToListAsync();

            for (var i = learningQuizzes.Count - 1; i >= 0; i--) {
                if (learningQuizzes[i].QuizId == null)
                    continue;

                var access = await _quizAccessesRepository.GetQuizAccessForUserAsync(userId, learningQuizzes[i].QuizId.Value);
                if (access == null || access.Access == QuizAccessEnum.None) {
                    learningQuizzes.RemoveAt(i);
                }
                else {
                    learningQuizzes[i].Quiz.IncludeAccess(access.Access);
                }
            }

            return await _auth0UsersService.IncludeOwnerNickNamesInLearningQuiz(learningQuizzes);
        }

        public async Task<IList<LearningQuiz>> GetAllLearningQuizInstancesOfQuizAsync(Guid quizId, string userId) {
            return await _learningQuizzesRepository
                .GetAll()
                .Where(l => !l.IsFinished)
                .Where(l => l.UserId == userId)
                .Where(l => l.QuizId == quizId)
                .ToListAsync();
        }

        public async Task<LearningQuiz> GetLearningQuizByIdAsync(long id, string userId) {
            var learningQuiz = await _learningQuizzesRepository
                .GetAllByUserId(userId)
                .Where(q => q.Id == id)
                .Include(q => q.Quiz)
                .SingleOrDefaultAsync();

            if (learningQuiz?.QuizId == null)
                return null;

            var isPublic = await _quizzesRepository.IsPublicAsync(learningQuiz.QuizId.Value);
            if (!isPublic) {
                var access = await _quizzesRepository.HaveReadAccessToQuizAsync(userId, learningQuiz.QuizId.Value);
                if (!access)
                    return null;
            }

            await _auth0UsersService.QuizItemWithOwnerNickName(learningQuiz.Quiz);
            return learningQuiz;
        }

        public async Task<object> NextQuestionAsync(long learningQuizId, string userId) {
            var learningQuiz = await _learningQuizzesRepository.GetById(learningQuizId);
            if (learningQuiz?.QuizId == null || learningQuiz.UserId != userId)
                return null;

            var isPublic = await _quizzesRepository.IsPublicAsync(learningQuiz.QuizId.Value);
            if (!isPublic) {
                var access = await _quizzesRepository.HaveReadAccessToQuizAsync(userId, learningQuiz.QuizId.Value);
                if (!access)
                    return null;
            }

            var maxVersionTime = learningQuiz.CreationTime;
            var reoccurrences = _learningQuizQuestionsRepository.GetAllByLearningQuizId(learningQuiz.Id);

            if (reoccurrences == null)
                return null;

            var remainingReoccurrences = reoccurrences.Where(q => q.Reoccurrences > 0);

            if (!remainingReoccurrences.Any()) {
                return new { IsFinished = true };
            }

            var randomQuestionReoccurrences = remainingReoccurrences.RandomElement();

            var randomQuestion = await _questionsRepository
                .GetAll()
                .Where(q => q.Id == randomQuestionReoccurrences.QuestionId)
                .Where(q => q.CreationTime <= maxVersionTime)
                .Include(q => q.Versions)
                .SingleOrDefaultAsync();

            randomQuestion.FlatVersionProps(maxVersionTime);

            var answers = await _answersRepository
                .GetAllByQuestionId(randomQuestion.Id, maxVersionTime, true) //TODO MIN VERSION TIME
                .Include(a => a.Versions)
                .Select(a => new {
                    a.Id,
                    a.Versions
                        .Where(b => b.CreationTime <= maxVersionTime)
                        .OrderByDescending(b => b.CreationTime)
                        .FirstOrDefault()
                        .Value
                })
                .ToListAsync();

            return new {
                IsFinished = false,
                randomQuestionReoccurrences.Reoccurrences,
                Question = randomQuestion,
                Answers = answers
            };
        }

        public async Task<LearningQuiz> CreateLearningQuizAsync(Guid quizId, string userId) {
            var isPublic = await _quizzesRepository.IsPublicAsync(quizId);
            if (!isPublic) {
                var access = await _quizzesRepository.HaveReadAccessToQuizAsync(userId, quizId);
                if (!access)
                    return null;
            }

            var creationTime = CurrentTime;

            var questionsQuery = _questionsRepository
                .GetAllByQuizId(quizId)
                .Where(q => q.CreationTime <= creationTime);

            var learningQuiz = new LearningQuiz {
                CreationTime = creationTime,
                UserId = userId,
                QuizId = quizId,
                NumberOfQuestions = await questionsQuery.CountAsync()
            };

            var settings = await _userSettingsRepository.GetByIdOrDefault(userId);
            var questions = questionsQuery
                .Select(q => new LearningQuizQuestion {
                    QuestionId = q.Id,
                    Reoccurrences = settings.ReoccurrencesOnStart
                });

            await _learningQuizzesRepository.Create(learningQuiz);
            _learningQuizzesRepository.AddQuestions(learningQuiz, questions);
            var result = await Context.SaveChangesAsync() > 0;
            return result ? learningQuiz : null;
        }

        public async Task<object> AnswerQuestion(long learningQuizId, LearningQuizUserAnswer userAnswer, string userId) {
            var learningQuiz = await _learningQuizzesRepository.GetById(learningQuizId);
            if (learningQuiz?.QuizId == null || learningQuiz.UserId != userId)
                return null;

            var questionReoccurrences = await _learningQuizQuestionsRepository
                .GetAllByLearningQuizId(learningQuizId: learningQuizId, asNoTracking: false)
                .Where(q => q.QuestionId == userAnswer.QuizQuestionId)
                .SingleOrDefaultAsync();

            if (questionReoccurrences == null)
                return null;

            var correctAnswers = await _answersRepository
                .GetAllByQuestionId(userAnswer.QuizQuestionId, learningQuiz.CreationTime, true)
                .Include(a => a.Versions)
                .Select(a => new {
                    a.Id,
                    a.Versions
                        .Where(b => b.CreationTime <= learningQuiz.CreationTime)
                        .OrderByDescending(b => b.CreationTime)
                        .FirstOrDefault()
                        .IsCorrect
                })
                .Where(a => a.IsCorrect)
                .Select(i => i.Id)
                .OrderBy(i => i)
                .ToListAsync();

            var selectedAnswers = userAnswer.SelectedAnswers
                .OrderBy(i => i)
                .ToList();

            var settings = await _userSettingsRepository.GetByIdOrDefault(userId);
            var isCorrect = correctAnswers.SequenceEqual(selectedAnswers);
            if (isCorrect) {
                ++learningQuiz.NumberOfCorrectAnswers;
                --questionReoccurrences.Reoccurrences;
                ++questionReoccurrences.GoodUserAnswers;
                if (questionReoccurrences.Reoccurrences == 0) {
                    learningQuiz.NumberOfLearnedQuestions = learningQuiz.NumberOfLearnedQuestions + 1;
                }
            }
            else {
                ++learningQuiz.NumberOfBadAnswers;
                questionReoccurrences.Reoccurrences += settings.ReoccurrencesIfBad;
                ++questionReoccurrences.BadUserAnswers;
                if (questionReoccurrences.Reoccurrences > settings.MaxReoccurrences)
                    questionReoccurrences.Reoccurrences = settings.MaxReoccurrences;
            }

            _learningQuizQuestionsRepository.Update(questionReoccurrences);
            _learningQuizzesRepository.Update(learningQuiz);
            await Context.SaveChangesAsync();

            var isUnfinished = await _learningQuizQuestionsRepository
                .GetAllByLearningQuizId(learningQuizId)
                .Where(q => q.Reoccurrences > 0)
                .AnyAsync();

            if (!isUnfinished) {
                await SetAsFinished(learningQuizId);
            }

            return new {
                IsFinished = !isUnfinished,
                IsCorrect = isCorrect,
                CorrectAnswers = correctAnswers,
                SelectedAnswers = selectedAnswers,
                questionReoccurrences.Reoccurrences,
                LearningQuiz = learningQuiz
            };
        }

        private async Task<bool> SetAsFinished(long id) {
            var quiz = await _learningQuizzesRepository.GetById(id);
            quiz.FinishedTime = CurrentTime;
            quiz.IsFinished = true;
            _learningQuizzesRepository.Update(quiz);
            return await Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteLearningQuizAsync(long id, string userId) {
            var haveOwnerAccess = await _learningQuizzesRepository
                .GetAllByUserId(userId)
                .Where(l => l.Id == id)
                .AnyAsync();

            if (!haveOwnerAccess)
                return false;

            await _learningQuizzesRepository.Delete(id);
            return await Context.SaveChangesAsync() > 0;
        }
    }
}
