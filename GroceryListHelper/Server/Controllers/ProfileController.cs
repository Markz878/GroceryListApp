using GroceryListHelper.DataAccess.Repositories;
using GroceryListHelper.Server.HelperMethods;
using GroceryListHelper.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GroceryListHelper.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly UserRepository userRepository;

        public ProfileController(UserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> LogOut()
        {
            int id = User.GetUserId();
            string response = await userRepository.RemoveRefreshToken(id);
            if (string.IsNullOrEmpty(response))
            {
                return Ok();
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            int id = User.GetUserId();
            string response = await userRepository.ChangePassword(id, request.CurrentPassword, request.NewPassword);
            if (string.IsNullOrEmpty(response))
            {
                return Ok();
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> ChangeEmail(ChangeEmailRequest request)
        {
            int id = User.GetUserId();
            string response = await userRepository.ChangeEmail(id, request.NewEmail, request.Password);
            if (string.IsNullOrEmpty(response))
            {
                return Ok();
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> Delete(DeleteProfileRequest request)
        {
            string response = await userRepository.DeleteUser(User.GetUserId(), request.Password);
            if (string.IsNullOrEmpty(response))
            {
                return Ok();
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserInfo()
        {
            UserModel user = await userRepository.GetUserFromId(User.GetUserId());
            if (user!=null)
            {
                return Ok(user);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
