using GroceryListHelper.Server.HelperMethods;
using GroceryListHelper.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GroceryListHelper.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly JWTAuthenticationManager authenticationManager;
        private readonly Random rng = new Random();

        public AuthenticationController(JWTAuthenticationManager authenticationManager)
        {
            this.authenticationManager = authenticationManager;
        }

        [HttpPost]
        public async Task<ActionResult<LoginResponseModel>> Register([FromBody] RegisterRequestModel user)
        {
            await Task.Delay(rng.Next(300, 700));
            string status = await authenticationManager.Register(user.Email, user.Password);
            if (!string.IsNullOrEmpty(status))
            {
                LoginResponseModel response = new() { Message = status };
                return BadRequest(response);
            }
            else
            {
                string accessToken = await authenticationManager.GetUserAccessToken(user.Email, user.Password);
                string refreshToken = await authenticationManager.GetUserRefreshToken(accessToken);
                if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
                {
                    LoginResponseModel response = new() { Message = "Access- or refreshtoken is invalid, please login." };
                    return BadRequest(response);
                }
                else
                {
                    LoginResponseModel response = new() { AccessToken = accessToken, RefreshToken = refreshToken };
                    return Ok(response);
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult<LoginResponseModel>> Login([FromBody] UserCredentialsModel user)
        {
            await Task.Delay(rng.Next(300, 700));
            string accessToken = await authenticationManager.GetUserAccessToken(user.Email, user.Password);
            string refreshToken = await authenticationManager.GetUserRefreshToken(accessToken);
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                LoginResponseModel response = new() { Message = "Invalid username or password" };
                return Unauthorized(response);
            }
            else
            {
                LoginResponseModel response = new() { AccessToken = accessToken, RefreshToken = refreshToken };
                return Ok(response);
            }
        }

        [HttpPost]
        public ActionResult<RefreshTokenResponseModel> Refresh(RefreshTokenRequestModel request)
        {
            string accessToken = authenticationManager.RefreshAccessToken(request.AccessToken, request.RefreshToken);
            if (!string.IsNullOrEmpty(accessToken))
            {
                RefreshTokenResponseModel response = new() { AccessToken = accessToken };
                return Ok(response);
            }
            RefreshTokenResponseModel errorResponse = new() { Message = "Invalid access- or refresh token" };
            return Unauthorized(errorResponse);
        }
    }
}
