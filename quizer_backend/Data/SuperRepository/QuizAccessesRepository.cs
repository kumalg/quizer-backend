using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities;
using quizer_backend.Data.Repository;

namespace quizer_backend.Data.SuperRepository {
    public class QuizAccessesRepository : GenericRepository<QuizAccess> {
        private readonly QuizerContext _context;

        public QuizAccessesRepository(QuizerContext context) : base(context) {
            _context = context;
        }

        public async Task<QuizAccess> GetQuizAccessForUser(string userId, long quizId) {
            return await _context.QuizAccessItems
                .Where(a => a.QuizId == quizId)
                .Where(a => a.UserId == userId)
                .SingleOrDefaultAsync();
        }
    }
}
