using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

        public async Task CreateOrUpdate(QuizAccess quizAccess) {
            var access = await GetQuizAccessForUserAsync(quizAccess.UserId, quizAccess.QuizId);
            if (access == null) {
                await Create(quizAccess);
                return;
            }
            access.Access = quizAccess.Access;
            Update(access);
        }

        public IQueryable<QuizAccess> GetAllByUserId(string userId) {
            return GetAll().Where(a => a.UserId == userId);
        }

        public async Task<EntityEntry> Delete(string userId, Guid quizId) {
            var entity = await GetQuizAccessForUserAsync(userId, quizId);
            return _context.Set<QuizAccess>().Remove(entity);
        }
    }
}
