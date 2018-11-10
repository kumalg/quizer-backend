using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities;

namespace quizer_backend.Data.Repository {
    public class QuizAccessesRepository : GenericRepository<QuizAccess> {
        private readonly QuizerContext _context;

        public QuizAccessesRepository(QuizerContext context) : base(context) {
            _context = context;
        }

        public async Task<QuizAccess> GetQuizAccessForUserAsync(string userId, Guid quizId) {
            return await GetAll()
                .Where(a => a.QuizId == quizId)
                .Where(a => a.UserId == userId)
                .SingleOrDefaultAsync();
        }

        public async Task<bool> Delete(string userId, Guid quizId) {
            var entity = await GetQuizAccessForUserAsync(userId, quizId);
            _context.Set<QuizAccess>().Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
