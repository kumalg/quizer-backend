using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using quizer_backend.Data.Entities;
using quizer_backend.Data.Repository;
using quizer_backend.Models;
using quizer_backend.Services;

namespace quizer_backend.Data.Services {
    public class UsersService : BaseService {

        private readonly AnonymousUsersRepository _anonymousUsersRepository;
        private readonly UserSettingsRepository _userSettingsRepository;
        private readonly QuizAccessesRepository _quizAccessesRepository;
        private readonly LearningQuizzesRepository _learningQuizzesRepository;
        private readonly SolvedQuizRepository _solvedQuizRepository;
        private readonly QuizzesRepository _quizzesRepository;

        public UsersService(QuizerContext context) : base(context) {
            _anonymousUsersRepository = new AnonymousUsersRepository(context);
            _userSettingsRepository = new UserSettingsRepository(context);
            _quizAccessesRepository = new QuizAccessesRepository(context);
            _learningQuizzesRepository = new LearningQuizzesRepository(context);
            _solvedQuizRepository = new SolvedQuizRepository(context);
            _quizzesRepository = new QuizzesRepository(context);
        }

        public async Task<string> GetAnonymousUserId(HttpRequest request) {
            var tokenString = request.Cookies["anonymous_user_id_token"];

            if (string.IsNullOrEmpty(tokenString))
                return null;

            var userId = AnonymousUsersService.GetUserId(tokenString);
            if (string.IsNullOrEmpty(userId))
                return "";
            var userGuid = Guid.Parse(userId);
            var exist = await _anonymousUsersRepository.ExistAsync(userGuid);
            return exist ? userId : "";
        }

        public async Task<string> GenerateAnonymousUserId(HttpResponse response) {
            var user = new AnonymousUser();
            await _anonymousUsersRepository.Create(user);
            await Context.SaveChangesAsync();
            var userId = user.Id.ToString();
            var token = AnonymousUsersService.GenerateTokenFromUserId(userId);
            response.Cookies.Append("anonymous_user_id_token", token, new CookieOptions {
                HttpOnly = true
            });
            return userId;
        }

        public async Task<UserSettings> GetUserSettingsByIdAsync(string userId) {
            return await _userSettingsRepository.GetByIdOrDefault(userId);
        }

        public async Task<UserSettings> UpdateUserSettingsAsync(NewUserSettings userSettings, string userId) {
            var settings = new UserSettings {
                UserId = userId,
                ReoccurrencesOnStart = userSettings.ReoccurrencesOnStart,
                ReoccurrencesIfBad = userSettings.ReoccurrencesIfBad,
                MaxReoccurrences = userSettings.MaxReoccurrences
            };
            await _userSettingsRepository.AddOrUpdate(userId, settings);
            var result = await Context.SaveChangesAsync() > 0;
            return result ? settings : null;
        }

        public async Task<bool> DeleteUserDataAsync(string userId) {
            var settings = await _userSettingsRepository.GetById(userId);
            if (settings != null)
                _userSettingsRepository.Delete(settings);

            var accesses = _quizAccessesRepository.GetAllByUserId(userId);
            _quizAccessesRepository.DeleteRange(accesses);
            
            var solvedQuizzes = _solvedQuizRepository.GetAllByUserId(userId);
            _solvedQuizRepository.DeleteRange(solvedQuizzes);

            var learningQuizzes = _learningQuizzesRepository.GetAllByUserId(userId);
            _learningQuizzesRepository.DeleteRange(learningQuizzes);

            var quizzes = _quizzesRepository.GetAllByUserId(userId);
            _quizzesRepository.DeleteRange(quizzes);

            return await Context.SaveChangesAsync() > 0;
        }
    }
}
