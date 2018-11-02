using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using quizer_backend.Data.Entities;
using quizer_backend.Data.Repository;
using quizer_backend.Models;

namespace quizer_backend.Controllers {

    [Route("user-settings")]
    public class UserSettingsController : QuizerApiControllerBase {

        private readonly UserSettingsRepository _userSettingsRepository;

        public UserSettingsController(UserSettingsRepository userSettingsRepository) {
            _userSettingsRepository = userSettingsRepository;
        }


        // GETOS

        [HttpGet]
        public async Task<UserSettings> GetUserSettings() {
            return await _userSettingsRepository.GetByIdOrDefault(UserId);
        }


        // PUTOS

        [HttpPut]
        public async Task<IActionResult> UpdateUserSettings(NewUserSettings userSettings) {
            var userId = UserId;
            var settings = new UserSettings {
                UserId = userId,
                ReoccurrencesOnStart = userSettings.ReoccurrencesOnStart,
                ReoccurrencesIfBad = userSettings.ReoccurrencesIfBad,
                MaxReoccurrences = userSettings.MaxReoccurrences
            };
            await _userSettingsRepository.AddOrUpdate(userId, settings);
            return Ok(settings);
        }
    }
}
