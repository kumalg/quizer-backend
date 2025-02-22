﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using quizer_backend.Data.Entities.QuizObject;

namespace quizer_backend.Data.Repository {
    public class AnswersRepository : GenericRepository<Answer> {
        private readonly QuizerContext _context;

        public AnswersRepository(QuizerContext context) : base(context) {
            _context = context;
        }

        //public async Task<bool> Create(Answer answer, AnswerVersion version) {
        //    await _context.Set<Answer>().AddAsync(answer);
        //    answer.Versions.Add(version);
        //    return await _context.SaveChangesAsync() > 0;
        //}

        public async Task<Answer> GetById(long id, bool allowDeleted = false) {
            if (!allowDeleted) {
                return await GetAll()
                    .Where(a => a.Id == id)
                    .Where(a => !a.IsDeleted)
                    .SingleOrDefaultAsync();
            }

            return await base.GetById(id);
        }

        public IQueryable<Answer> GetAllByQuestionId(long questionId, long? maxVersionTime = null, bool allowDeleted = false) {
            var query = GetAll().Where(q => q.QuestionId == questionId);

            if (!allowDeleted)
                query = query.Where(a => !a.IsDeleted);

            if (maxVersionTime != null) {
                query = query.Where(q => q.CreationTime <= maxVersionTime);

                if (allowDeleted) {
                    query = query.Where(q => q.DeletionTime == null || q.DeletionTime.Value > maxVersionTime);
                }
            }

            return query;
        }

        public async Task<Guid> GetQuizId(long answerId) {
            return await GetAll()
                .Where(a => a.Id == answerId)
                .Include(a => a.Question)
                .Select(a => a.Question.QuizId)
                .SingleOrDefaultAsync();
        }

        public async Task<EntityEntry> SilentDelete(long id, long timestamp) {
            var entity = await GetById(id);
            entity.IsDeleted = true;
            entity.DeletionTime = timestamp;
            return Update(entity);
        }
    }
}
