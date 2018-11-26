using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Repository;

namespace quizer_backend.Data.Services {
    public class QuestionsService : BaseService {

        private readonly QuizzesRepository _quizzesRepository;
        private readonly QuestionsRepository _questionsRepository;
        private readonly AnswersRepository _answersRepository;

        public QuestionsService(QuizerContext context) : base(context) {
            _quizzesRepository = new QuizzesRepository(context);
            _questionsRepository = new QuestionsRepository(context);
            _answersRepository = new AnswersRepository(context);
        }

        
        // Read methods

        public async Task<IList<Answer>> GetQuestionAnswersAsync(long questionId, string userId, long? maxVersionTime = null) {
            var question = await _questionsRepository.GetById(questionId);

            var access = await _quizzesRepository.HaveReadAccessToQuizAsync(userId, question.QuizId);
            if (!access)
                return null;

            return await _answersRepository
                .GetAllByQuestionId(questionId, maxVersionTime)
                .Include(a => a.Versions)
                .Select(a => a.FlatVersionProps(maxVersionTime))
                .ToListAsync();
        }


        // Create/Update/Delete methods

        public async Task<Question> CreateQuestionAsync(Question question, string userId) {
            var access = await _quizzesRepository.HaveWriteAccessToQuiz(userId, question.QuizId);
            if (!access)
                return null;

            var creationTime = CurrentTime;
            question.CreationTime = creationTime;

            var questionVersion = new QuestionVersion {
                CreationTime = creationTime,
                Value = question.Value
            };

            question.Versions.Add(questionVersion);
            question.FlatVersionProps();

            await _questionsRepository.Create(question);
            return await Context.SaveChangesAsync() > 0
                ? question
                : null;
        }

        public async Task<Question> UpdateQuestionAsync(long questionId, string value, string userId) {
            var question = await _questionsRepository
                .GetAll()
                .Where(a => a.Id == questionId)
                .Where(a => !a.IsDeleted)
                .SingleOrDefaultAsync();

            if (question == null)
                return null;

            var access = await _quizzesRepository.HaveWriteAccessToQuiz(userId, question.QuizId);
            if (!access)
                return null;

            var questionVersion = new QuestionVersion {
                CreationTime = CurrentTime,
                Value = value
            };
            question.Versions.Add(questionVersion);
            _questionsRepository.Update(question);
            var result = await Context.SaveChangesAsync() > 0;

            if (!result)
                return null;

            question.FlatVersionProps();
            return question;
        }

        public async Task<bool> DeleteQuestionAsync(long questionId, string userId) {
            var question = await _questionsRepository.GetById(questionId);

            var haveOwnerAccess = await _quizzesRepository.HaveOwnerAccessToQuiz(userId, question.QuizId);
            if (!haveOwnerAccess)
                return false;

            await _questionsRepository.SilentDelete(questionId, CurrentTime);
            return await Context.SaveChangesAsync() > 0;
        }
    }
}
