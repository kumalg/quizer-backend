using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using quizer_backend.Data.Entities;
using quizer_backend.Data.Services;
using quizer_backend.Models;

namespace quizer_backend.Controllers {

    [Route("user-settings")]
    public class UserSettingsController : QuizerApiControllerBase {

        private readonly UsersService _usersService;

        public UserSettingsController(UsersService usersService) {
            _usersService = usersService;
        }


        // GETOS

        [HttpGet]
        public async Task<UserSettings> GetUserSettings() {
            return await _usersService.GetUserSettingsByIdAsync(UserId);
        }


        // PUTOS

        [HttpPut]
        public async Task<IActionResult> UpdateUserSettings(NewUserSettings userSettings) {
            var settings = await _usersService.UpdateUserSettingsAsync(userSettings, UserId);
            if (settings == null)
                return BadRequest();
            return Ok(settings);
        }
    }
}
