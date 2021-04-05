using GroceryListHelper.Server.HelperMethods;
using GroceryListHelper.Shared;
using Microsoft.AspNetCore.Http;
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
        private readonly CookieBuilder cookieOptions = new() { Expiration = TimeSpan.FromDays(1), HttpOnly = true, SecurePolicy = CookieSecurePolicy.Always, IsEssential = true, SameSite = SameSiteMode.Strict, MaxAge = TimeSpan.FromDays(1) };

        public AuthenticationController(JWTAuthenticationManager authenticationManager)
        {
            this.authenticationManager = authenticationManager;
        }

        [HttpPost]
        public async Task<ActionResult<LoginResponseModel>> Register([FromBody] RegisterRequestModel user)
        {
            string error = await authenticationManager.Register(user.Email, user.Password);
            if (!string.IsNullOrEmpty(error))
            {
                LoginResponseModel response = new() { Message = error };
                return BadRequest(response);
            }
            else
            {
                string accessToken = await authenticationManager.GetUserAccessToken(user.Email, user.Password);
                string refreshToken = await authenticationManager.GetUserRefreshToken(accessToken);
                if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
                {
                    Response.Cookies.Append(GlobalConstants.XRefreshToken, refreshToken, cookieOptions.Build(HttpContext));
                    LoginResponseModel response = new() { AccessToken = accessToken };
                    return Ok(response);
                }
                LoginResponseModel errorResponse = new() { Message = "Access- or refreshtoken is invalid, please login." };
                return Unauthorized(errorResponse);
            }
        }

        [HttpPost]
        public async Task<ActionResult<LoginResponseModel>> Login([FromBody] UserCredentialsModel user)
        {
            string accessToken = await authenticationManager.GetUserAccessToken(user.Email, user.Password);
            string refreshToken = await authenticationManager.GetUserRefreshToken(accessToken);
            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                Response.Cookies.Append(GlobalConstants.XRefreshToken, refreshToken, cookieOptions.Build(HttpContext));
                LoginResponseModel response = new() { AccessToken = accessToken };
                return Ok(response);
            }
            LoginResponseModel errorResponse = new() { Message = "Invalid username or password" };
            return Unauthorized(errorResponse);
        }

        [HttpGet]
        public async Task<ActionResult<LoginResponseModel>> Refresh()
        {
            if (Request.Cookies.TryGetValue(GlobalConstants.XRefreshToken, out string refreshToken))
            {
                string accessToken = authenticationManager.RefreshAccessToken(refreshToken);
                string newRefreshToken = await authenticationManager.RenewRefreshToken(refreshToken);
                if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(newRefreshToken))
                {
                    Response.Cookies.Append(GlobalConstants.XRefreshToken, newRefreshToken, cookieOptions.Build(HttpContext));
                    LoginResponseModel response = new() { AccessToken = accessToken };
                    return Ok(response);
                }
            }
            LoginResponseModel errorResponse = new() { Message = "Invalid access- or refresh token" };
            return Unauthorized(errorResponse);
        }
    }
}
