﻿using quizer_backend.Data.Entities.QuizObject;

namespace quizer_backend.Data.Repository {
    public class QuestionVersionsRepository : GenericRepository<QuestionVersion> {
        private readonly QuizerContext _context;

        public QuestionVersionsRepository(QuizerContext context) : base(context) {
            _context = context;
        }
    }
}
