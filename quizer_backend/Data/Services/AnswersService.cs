using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Data.Repository;

namespace quizer_backend.Data.Services {
    public class AnswersService : BaseService {

        private readonly QuizzesRepository _quizzesRepository;
        private readonly QuestionsRepository _questionsRepository;
        private readonly AnswersRepository _answersRepository;

        public AnswersService(QuizerContext context) : base(context) {
            _quizzesRepository = new QuizzesRepository(context);
            _questionsRepository = new QuestionsRepository(context);
            _answersRepository = new AnswersRepository(context);
        }


        // Create/Update/Delete methods

        public async Task<Answer> CreateAnswerAsync(Answer answer, string userId) {
            var question = await _questionsRepository.GetById(answer.QuestionId);
            if (question == null)
                return null;

            var access = await _quizzesRepository.HaveWriteAccessToQuiz(userId, question.QuizId);
            if (!access)
                return null;

            var creationTime = CurrentTime;
            answer.CreationTime = creationTime;

            var version = new AnswerVersion {
                CreationTime = creationTime,
                Value = answer.Value,
                IsCorrect = answer.IsCorrect
            };
            answer.Versions.Add(version);
            answer.FlatVersionProps();

            await _answersRepository.Create(answer);
            var result = await Context.SaveChangesAsync() > 0;
            return result ? answer : null;
        }


        // PUTOS

        public async Task<Answer> UpdateAnswerAsync(Answer newAnswer, string userId) {
            if (string.IsNullOrEmpty(newAnswer.Value))
                return null;

            var answer = await _answersRepository
                .GetAll()
                .Where(a => a.Id == newAnswer.Id)
                .Where(a => !a.IsDeleted)
                .Include(a => a.Question)
                .SingleOrDefaultAsync();

            if (answer == null)
                return null;

            var access = await _quizzesRepository.HaveWriteAccessToQuiz(userId, answer.Question.QuizId);
            if (!access)
                return null;

            var answerVersion = new AnswerVersion {
                CreationTime = CurrentTime,
                Value = newAnswer.Value,
                IsCorrect = newAnswer.IsCorrect
            };

            answer.Versions.Add(answerVersion);
            _answersRepository.Update(answer);
            var result = await Context.SaveChangesAsync() > 0;

            if (!result)
                return null;

            answer.FlatVersionProps();
            return answer;
        }


        // DELETOS
        
        public async Task<bool> DeleteAnswerAsync(long answerId, string userId) {
            var quizId = await _answersRepository.GetQuizId(answerId);

            var access = await _quizzesRepository.HaveWriteAccessToQuiz(userId, quizId);
            if (!access)
                return false;

            await _answersRepository.SilentDelete(answerId, CurrentTime);
            return await Context.SaveChangesAsync() > 0;
        }
    }
}
