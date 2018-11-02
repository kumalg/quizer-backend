using System.Threading.Tasks;
using quizer_backend.Data.Entities;

namespace quizer_backend.Data.Repository {
    public class UserSettingsRepository : GenericRepository<UserSettings> {
        private readonly QuizerContext _context;

        public UserSettingsRepository(QuizerContext context) : base(context) {
            _context = context;
        }

        public async Task<UserSettings> GetByIdOrDefault(string userId) {
            return await GetById(userId) ?? new UserSettings();
        }

        public async Task<UserSettings> AddOrUpdate(string userId, UserSettings userSettings) {
            var currentSettings = await GetById(userId);
            if (currentSettings == null) {
                await Create(userSettings);
            }
            else {
                currentSettings.ReoccurrencesIfBad = userSettings.ReoccurrencesIfBad;
                currentSettings.ReoccurrencesOnStart = userSettings.ReoccurrencesOnStart;
                currentSettings.MaxReoccurrences = userSettings.MaxReoccurrences;

                await Update(userId, currentSettings);
            }
            return userSettings;
        }
    }
}
