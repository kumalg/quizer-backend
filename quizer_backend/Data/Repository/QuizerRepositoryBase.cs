using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities;
using quizer_backend.Data.Entities.QuizObject;

namespace quizer_backend.Data.Repository {
    public class QuizerRepositoryBase {
        protected readonly QuizerContext Context;

        public QuizerRepositoryBase(QuizerContext context) {
            Context = context;
        }

        public async Task<bool> SaveAllAsync() {
            return (await Context.SaveChangesAsync()) > 0;
        }

        public async Task<QuizAccess> UserAccessToQuizAsync(string userId, long quizId) {
            return await Context.QuizAccessItems
                .Where(i => i.QuizId == quizId)
                .Where(i => i.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<Quiz> IncludeAccess(Quiz quiz, string userId) {
            var access = await UserAccessToQuizAsync(userId, quiz.Id);
            return quiz.IncludeAccess(access.Access);
        }
    }
}
