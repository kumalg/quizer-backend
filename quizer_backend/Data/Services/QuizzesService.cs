using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Repository;
using quizer_backend.Models;
using quizer_backend.Services;

namespace quizer_backend.Data.Services {
    public class QuizzesService : BaseService {

        private readonly QuizzesRepository _quizzesRepository;
        private readonly QuestionsRepository _questionsRepository;
        private readonly QuizAccessesRepository _quizAccessesRepository;
        private readonly Auth0UsersService _auth0UsersService;

        public QuizzesService(
            QuizerContext context,
            Auth0ManagementFactory auth0ManagementFactory
        ) : base(context) {
            _quizzesRepository = new QuizzesRepository(context);
            _questionsRepository = new QuestionsRepository(context);
            _quizAccessesRepository = new QuizAccessesRepository(context);
            _auth0UsersService = new Auth0UsersService(auth0ManagementFactory);
        }


        // Read methods

        public async Task<IEnumerable<Quiz>> GetUserQuizzesAsync(string userId) {
            var quizzes = await _quizzesRepository
                .GetAllByUserId(userId)
                .ToListAsync();

            await IncludeUserAccessAsync(quizzes, userId);

            var quizzesWithOwners = await _auth0UsersService.IncludeOwnerNickNames(quizzes);
            return quizzesWithOwners;
        }

        public async Task<bool> IsQuizAttachedToUserAsync(Guid quizId, string userId) {
            return await _quizAccessesRepository
                .GetAll()
                .Where(a => a.UserId == userId)
                .Where(a => a.QuizId == quizId)
                .Where(a => a.Access != QuizAccessEnum.None)
                .AnyAsync();
        }

        public async Task<Quiz> GetByIdAsync(Guid quizId, string userId) {
            var access = await _quizzesRepository.HaveReadAccessToQuizAsync(userId, quizId);
            if (!access)
                return null;

            var quiz = await _quizzesRepository.GetById(quizId);

            var userAccess = await _quizAccessesRepository.GetQuizAccessForUserAsync(userId, quizId);
            quiz.IncludeAccess(userAccess.Access);

            var quizWithOwnerNickName = await _auth0UsersService.QuizItemWithOwnerNickName(quiz);
            return quizWithOwnerNickName;
        }

        public async Task<IList<FlatQuestion>> QuestionsInQuizAsync(Guid quizId, string userId, long? maxTime = null) {
            var access = await _quizzesRepository.HaveReadAccessToQuizAsync(userId, quizId);
            if (!access)
                return null;

            return await _questionsRepository
                .GetAllByQuizId(quizId, maxTime)
                .Include(q => q.Versions)
                .Select(q => q.FlatVersionProps(maxTime))
                .Select(q => new FlatQuestion {
                    Id = q.Id,
                    QuizId = q.QuizId,
                    Value = q.Value,
                    CreationTime = q.CreationTime
                })
                .ToListAsync();
        }


        // Create/Update/Delete methods

        public async Task<bool> CreateQuizAsync(Quiz quiz, string userId) {
            var creationTime = CurrentTime;
            quiz.CreationTime = creationTime;
            quiz.LastModifiedTime = creationTime;
            quiz.OwnerId = userId;

            quiz.Accesses.Add(new QuizAccess {
                Access = QuizAccessEnum.Owner,
                UserId = userId
            });

            await _quizzesRepository.Create(quiz); //TODO not sure if after access
            return await Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> GiveAccess(Guid quizId, string userId, string email, QuizAccessEnum accessType) {
            var haveOwnerAccess = await _quizzesRepository.HaveOwnerAccessToQuiz(userId, quizId);
            if (!haveOwnerAccess)
                return false;
            
            var users = await _auth0UsersService.GetUsersByEmailAsync(email);
            if (users == null || users.Count == 0)
                return false;

            foreach (var user in users) {
                var access = new QuizAccess {
                    Access = accessType,
                    QuizId = quizId,
                    UserId = user.UserId
                };
                await _quizAccessesRepository.Create(access);
            }

            return await Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> StopCreating(Guid quizId, string userId) {
            var access = await _quizAccessesRepository.GetQuizAccessForUserAsync(userId, quizId);
            if (access == null || access.Access != QuizAccessEnum.Creator)
                return false;

            access.Access = QuizAccessEnum.Solver;
            _quizAccessesRepository.Update(access);

            return await Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveAccess(Guid quizId, string userId) {
            await _quizAccessesRepository.Delete(userId, quizId);
            return await Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddAccess(Guid quizId, string userId) {
            var isPublic = await _quizzesRepository.IsPublicAsync(quizId);
            if (!isPublic)
                return false;

            var access = new QuizAccess {
                UserId = userId,
                QuizId = quizId,
                Access = QuizAccessEnum.Solver
            };
            await _quizAccessesRepository.Create(access);
            return await Context.SaveChangesAsync() > 0;
        }

        public async Task<Quiz> UpdateQuizAsync(Quiz newQuiz, string userId) {
            if (string.IsNullOrEmpty(newQuiz.Name))
                return null; //TODO exception

            var haveOwnerAccess = await _quizzesRepository.HaveOwnerAccessToQuiz(userId, newQuiz.Id);
            if (!haveOwnerAccess)
                return null;

            var quiz = await _quizzesRepository.GetById(newQuiz.Id);
            if (quiz == null)
                return null;

            quiz.Name = newQuiz.Name;
            quiz.QuestionsInSolvingQuiz = newQuiz.QuestionsInSolvingQuiz;
            quiz.MinutesInSolvingQuiz = newQuiz.MinutesInSolvingQuiz;
            quiz.IsPublic = newQuiz.IsPublic;

            _quizzesRepository.Update(quiz);
            var result = await Context.SaveChangesAsync() > 0;
            return result ? quiz : null;
        }

        public async Task<bool> DeleteQuiz(Guid quizId, string userId) {
            var haveOwnerAccess = await _quizzesRepository.HaveOwnerAccessToQuiz(userId, quizId);
            if (!haveOwnerAccess)
                return false;
            await _quizzesRepository.Delete(quizId);
            return await Context.SaveChangesAsync() > 0;
        }


        // PRIVATE HELPEROS

        private async Task IncludeUserAccessAsync(IEnumerable<Quiz> quizzes, string userId) {
            foreach (var quiz in quizzes) {
                var userAccess = await _quizAccessesRepository.GetQuizAccessForUserAsync(userId, quiz.Id);
                quiz.IncludeAccess(userAccess.Access);
            }
        }
    }

    public class FlatQuestion {
        public long Id { get; set; }
        public Guid QuizId { get; set; }
        public string Value { get; set; }
        public long CreationTime { get; set; }
    }
}
