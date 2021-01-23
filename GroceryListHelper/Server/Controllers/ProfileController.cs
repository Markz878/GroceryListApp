using GroceryListHelper.Server.HelperMethods;
using GroceryListHelper.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GroceryListHelper.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly JWTAuthenticationManager authenticationManager;

        public ProfileController(JWTAuthenticationManager authenticationManager)
        {
            this.authenticationManager = authenticationManager;
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            string email = User.FindFirst(ClaimTypes.Email).Value;
            string response = await authenticationManager.LogOut(email);
            if (string.IsNullOrEmpty(response))
            {
                return Ok();
            }
            else
            {
                return Unauthorized(response);
            }
        }

        [HttpPatch]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            string email = User.FindFirst(ClaimTypes.Email).Value;
            string response = await authenticationManager.ChangePassword(email, request.CurrentPassword, request.NewPassword);
            if (string.IsNullOrEmpty(response))
            {
                return Ok();
            }
            else
            {
                return Unauthorized(response);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(DeleteProfileRequest request)
        {
            string response = await authenticationManager.DeleteUser(User.FindFirstValue(ClaimTypes.Email), request.Password);
            if (string.IsNullOrEmpty(response))
            {
                return Ok();
            }
            else
            {
                return Unauthorized(response);
            }
        }
    }
}
