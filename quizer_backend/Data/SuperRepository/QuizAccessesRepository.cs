using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities;

namespace quizer_backend.Data.SuperRepository {
    public class QuizAccessesRepository : GenericRepository<QuizAccess> {
        private readonly QuizerContext _context;

        public QuizAccessesRepository(QuizerContext context) : base(context) {
            _context = context;
        }

        public async Task<QuizAccess> GetQuizAccessForUserAsync(string userId, long quizId) {
            return await GetAll()
                .Where(a => a.QuizId == quizId)
                .Where(a => a.UserId == userId)
                .SingleOrDefaultAsync();
        }
    }
}
