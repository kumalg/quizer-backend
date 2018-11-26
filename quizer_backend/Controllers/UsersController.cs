using System;
using System.Threading.Tasks;
using Auth0.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using quizer_backend.Data;
using quizer_backend.Data.Services;
using quizer_backend.Services;

namespace quizer_backend.Controllers {

    [Route("users")]
    public class UsersController : QuizerApiControllerBase {

        private readonly Auth0UsersService _auth0UsersService;
        private readonly UsersService _usersService;

        public UsersController(QuizerContext context, Auth0ManagementFactory auth0ManagementFactory) {
            _auth0UsersService = new Auth0UsersService(auth0ManagementFactory);
            _usersService = new UsersService(context);
        }


        // PATCHOS

        [HttpPatch("update-password")]
        public async Task<IActionResult> UpdatePasswordAsync(string newPassword, string newPasswordConfirmation) {
            if (newPassword != newPasswordConfirmation) {
                return BadRequest("passwords don't match");
            }

            try {
                var updated = await _auth0UsersService.UpdatePasswordAsync(UserId, newPassword);
                if (!updated)
                    return BadRequest();
                return Ok();
            }
            catch (ApiException e) {
                return BadRequest(e.ApiError);
            }
            catch (Exception e) {
                return BadRequest(e.Message);
            }
        }


        // DELETOS

        [HttpDelete]
        public async Task<IActionResult> DeleteUserAsync() {
            var userId = UserId;
            try {
                await _auth0UsersService.DeleteUserAsync(userId);
                await _usersService.DeleteUserDataAsync(userId);
                return Ok();
            }
            catch (ApiException e) {
                return BadRequest(e.ApiError);
            }
            catch (Exception e) {
                return BadRequest(e.Message);
            }
        }
    }
}
