using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using quizer_backend.Data.Entities.LearningQuiz;
using quizer_backend.Data.Entities.QuizObject;
using quizer_backend.Models;

namespace quizer_backend.Services {
    public class Auth0UsersService {

        private readonly Auth0ManagementFactory _auth0ManagementFactory;

        private async Task<ManagementApiClient> GetManagementApiClientAsync()
            => await _auth0ManagementFactory.GetManagementApiClientAsync();

        public Auth0UsersService(Auth0ManagementFactory auth0ManagementApiClient) {
            _auth0ManagementFactory = auth0ManagementApiClient;
        }

        public async Task<IEnumerable<Quiz>> IncludeOwnerNickNames(IList<Quiz> quizzes) {
            var userIds = quizzes
                .Select(q => q.OwnerId)
                .Distinct()
                .ToList();

            if (!userIds.Any())
                return quizzes;

            var search = new GetUsersRequest {
                SearchEngine = "v3",
                Query = $"user_id: ({string.Join(" OR ", userIds)})"
            };
            var client = await GetManagementApiClientAsync();
            var owners = await client.Users.GetAllAsync(search);

            return from quiz in quizzes
                   join owner in owners on quiz.OwnerId equals owner.UserId into users
                   from user in users.DefaultIfEmpty()
                   select quiz.IncludeOwner(user);
        }
    
        public async Task<IEnumerable<BasicUser>> GetBasicUsersListByIds(IList<string> ids) {
            if (!ids.Any())
                return new List<BasicUser>();

            var search = new GetUsersRequest {
                SearchEngine = "v3",
                Query = $"user_id: ({string.Join(" OR ", ids)})"
            };
            var client = await GetManagementApiClientAsync();
            var users = await client.Users.GetAllAsync(search);

            return users.Select(u => new BasicUser {
                UserId = u.UserId,
                NickName = u.NickName,
                Email = u.Email,
                Picture = u.Picture
            });
        }

        public async Task<IEnumerable<LearningQuiz>> IncludeOwnerNickNamesInLearningQuiz(IList<LearningQuiz> quizzes) {
            var userIds = quizzes
                .Where(q => q.QuizId != null)
                .Select(q => q.Quiz.OwnerId)
                .Distinct()
                .ToList();

            if (!userIds.Any())
                return quizzes;

            var search = new GetUsersRequest {
                SearchEngine = "v3",
                Query = $"user_id: ({string.Join(" OR ", userIds)})"
            };
            var client = await GetManagementApiClientAsync();
            var owners = await client.Users.GetAllAsync(search);

            var yco = from quiz in quizzes
                where quiz.QuizId != null
                join owner in owners on quiz.Quiz.OwnerId equals owner.UserId into users
                from user in users.DefaultIfEmpty()
                select quiz.IncludeOwnerInQuiz(user);

            return yco.Concat(quizzes.Where(q => q.QuizId == null));
        }

        public async Task<Quiz> QuizItemWithOwnerNickName(Quiz quiz) {
            var client = await _auth0ManagementFactory.GetManagementApiClientAsync();

            User owner = null;
            try {
                owner = await client.Users.GetAsync(quiz.OwnerId);
            }
            catch {

            }

            if (owner != null)
                quiz.IncludeOwner(owner);

            return quiz;
        }

        public async Task<IList<User>> GetUsersByEmailAsync(string email) {
            var client = await GetManagementApiClientAsync();
            return await client.Users.GetUsersByEmailAsync(email);
        }

        public async Task DeleteUserAsync(string userId) {
            var client = await GetManagementApiClientAsync();
            await client.Users.DeleteAsync(userId);
        }

        public async Task<bool> UpdatePasswordAsync(string userId, string newPassword) {
            var client = await GetManagementApiClientAsync();
            var userUpdateRequest = new UserUpdateRequest {
                Password = newPassword
            };
            var user = await client.Users.UpdateAsync(userId, userUpdateRequest);
            return user != null;
        }
    }
}
