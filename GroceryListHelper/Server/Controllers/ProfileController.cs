using GroceryListHelper.DataAccess.Repositories;
using GroceryListHelper.Server.HelperMethods;
using GroceryListHelper.Shared;
using Microsoft.AspNetCore.Authorization;
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

        [HttpDelete]
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
    }
}
