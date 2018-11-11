using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities;

namespace quizer_backend.Data.Repository {
    public class AnonymousUsersRepository : GenericRepository<AnonymousUser> {
        private readonly QuizerContext _context;

        public AnonymousUsersRepository(QuizerContext context) : base(context) {
            _context = context;
        }

        public async Task<bool> ExistAsync(Guid userId) {
            return await GetAll()
                .Where(i => i.Id == userId)
                .AnyAsync();
        }
    }
}
