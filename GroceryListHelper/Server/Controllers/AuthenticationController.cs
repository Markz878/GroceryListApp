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
        private readonly Random rng = new Random();
        private readonly CookieBuilder cookieOptions = new CookieBuilder() { Expiration = TimeSpan.FromDays(1), HttpOnly = true, SecurePolicy = CookieSecurePolicy.Always, IsEssential = true, SameSite = SameSiteMode.Strict, MaxAge = TimeSpan.FromDays(1) };

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
                if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
                {
                    Response.Cookies.Append(GlobalConstants.XRefreshToken, refreshToken, cookieOptions.Build(HttpContext));
                    LoginResponseModel response = new() { AccessToken = accessToken };
                    return Ok(response);
                }
                else
                {
                    LoginResponseModel response = new() { Message = "Access- or refreshtoken is invalid, please login." };
                    return BadRequest(response);
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult<LoginResponseModel>> Login([FromBody] UserCredentialsModel user)
        {
            await Task.Delay(rng.Next(300, 700));
            string accessToken = await authenticationManager.GetUserAccessToken(user.Email, user.Password);
            string refreshToken = await authenticationManager.GetUserRefreshToken(accessToken);
            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                Response.Cookies.Append(GlobalConstants.XRefreshToken, refreshToken, cookieOptions.Build(HttpContext));
                LoginResponseModel response = new() { AccessToken = accessToken };
                return Ok(response);
            }
            else
            {
                LoginResponseModel response = new() { Message = "Invalid username or password" };
                return Unauthorized(response);
            }
        }

        [HttpGet]
        public async Task<ActionResult<LoginResponseModel>> Refresh()
        {
            if (Request.Cookies.TryGetValue(GlobalConstants.XRefreshToken, out string refreshToken))
            {
                string accessToken = authenticationManager.RefreshAccessToken(refreshToken);
                string newRefreshToken = await authenticationManager.RenewRefreshToken(refreshToken);
                if (!string.IsNullOrEmpty(accessToken) || !string.IsNullOrEmpty(accessToken))
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
