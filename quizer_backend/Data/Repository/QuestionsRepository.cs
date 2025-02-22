﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using quizer_backend.Data.Entities.QuizObject;

namespace quizer_backend.Data.Repository {
    public class QuestionsRepository : GenericRepository<Question> {
        private readonly QuizerContext _context;

        public QuestionsRepository(QuizerContext context) : base(context) {
            _context = context;
        }

        //public async Task<bool> Create(Question question, QuestionVersion version) {
        //    await _context.Set<Question>().AddAsync(question);
        //    question.Versions.Add(version);
        //    return await _context.SaveChangesAsync() > 0;
        //}

        public async Task<Question> GetById(long id, bool allowDeleted = false) {
            if (!allowDeleted) {
                return await GetAll()
                    .Where(a => a.Id == id)
                    .Where(a => !a.IsDeleted)
                    .SingleOrDefaultAsync();
            }

            return await base.GetById(id);
        }

        public IQueryable<Question> GetAllByQuizId(Guid quizId, long? maxVersionTime = null, bool allowDeleted = false) {
            var query = GetAll().Where(q => q.QuizId == quizId);

            if (!allowDeleted)
                query = query.Where(q => !q.IsDeleted);

            if (maxVersionTime != null) {
                query = query.Where(q => q.CreationTime <= maxVersionTime);
                
                if (allowDeleted) {
                    query = query.Where(q => q.DeletionTime == null || q.DeletionTime.Value > maxVersionTime);
                }
            }

            return query;
        }

        public async Task<EntityEntry> SilentDelete(long id, long timestamp) {
            var entity = await GetById(id);
            entity.IsDeleted = true;
            entity.DeletionTime = timestamp;
            return Update(entity);
        }
    }
}
