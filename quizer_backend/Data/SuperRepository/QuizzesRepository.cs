using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Repository;
using quizer_backend.Models;

namespace quizer_backend.Data.SuperRepository {
    public class QuizzesRepository : GenericRepository<Quiz> {
        private readonly QuizerContext _context;

        public QuizzesRepository(QuizerContext context) : base(context) {
            _context = context;
        }

        public IQueryable<Quiz> GetAllByUserId(string userId) {
            return _context.QuizAccessItems
                .Where(a => a.UserId == userId)
                .Where(a => a.Access != QuizAccessEnum.None)
                .Include(a => a.Quiz)
                .Select(a => a.Quiz);
        }

        public async Task<bool> HaveReadAccessToQuiz(string userId, long quizId) {
            return await _context.QuizAccessItems
                .Where(a => a.UserId == userId)
                .Where(a => a.QuizId == quizId)
                .Where(a => a.Access != QuizAccessEnum.None)
                .AnyAsync();
        }

        public async Task<bool> HaveWriteAccessToQuiz(string userId, long quizId) {
            return await _context.QuizAccessItems
                .Where(a => a.UserId == userId)
                .Where(a => a.QuizId == quizId)
                .Where(a => a.Access == QuizAccessEnum.Creator || a.Access == QuizAccessEnum.Owner)
                .AnyAsync();
        }

        public async Task<bool> HaveOwnerAccessToQuiz(string userId, long quizId) {
            return await _context.QuizAccessItems
                .Where(a => a.UserId == userId)
                .Where(a => a.QuizId == quizId)
                .Where(a => a.Access == QuizAccessEnum.Owner)
                .AnyAsync();
        }
    }
}
