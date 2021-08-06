using GroceryListHelper.Server.HelperMethods;
using GroceryListHelper.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace GroceryListHelper.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly JWTAuthenticationManager authenticationManager;

        public AuthenticationController(IConfiguration configuration, JWTAuthenticationManager authenticationManager)
        {
            this.configuration = configuration;
            this.authenticationManager = authenticationManager;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticationResponseModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthenticationResponseModel))]
        public async Task<ActionResult<AuthenticationResponseModel>> Register([FromBody] RegisterRequestModel user)
        {
            (AuthenticationResponseModel response, string refreshToken) = await authenticationManager.Register(user.Email, user.Password);
            if (string.IsNullOrEmpty(response.ErrorMessage) && !string.IsNullOrEmpty(response.AccessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                Response.Cookies.Append(GlobalConstants.XRefreshToken, refreshToken, GetCookieOptions(HttpContext));
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticationResponseModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthenticationResponseModel))]
        public async Task<ActionResult<AuthenticationResponseModel>> Login([FromBody] UserCredentialsModel user)
        {
            (AuthenticationResponseModel response, string refreshToken) = await authenticationManager.Login(user.Email, user.Password);
            if (string.IsNullOrEmpty(response.ErrorMessage) && !string.IsNullOrEmpty(response.AccessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                Response.Cookies.Append(GlobalConstants.XRefreshToken, refreshToken, GetCookieOptions(HttpContext));
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticationResponseModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthenticationResponseModel))]
        public async Task<ActionResult<AuthenticationResponseModel>> Refresh()
        {
            if (Request.Cookies.TryGetValue(GlobalConstants.XRefreshToken, out string refreshToken))
            {
                (AuthenticationResponseModel response, string newRefreshToken) = await authenticationManager.RefreshTokens(refreshToken);
                if (string.IsNullOrEmpty(response.ErrorMessage) && !string.IsNullOrEmpty(response.AccessToken) && !string.IsNullOrEmpty(newRefreshToken))
                {
                    Response.Cookies.Append(GlobalConstants.XRefreshToken, newRefreshToken, GetCookieOptions(HttpContext));
                    return Ok(response);
                }
                return BadRequest(response);
            }
            AuthenticationResponseModel errorResponse = new() { ErrorMessage = "No refresh token in cookies." };
            return BadRequest(errorResponse);
        }

        private CookieOptions GetCookieOptions(HttpContext httpContext)
        {
            return new CookieBuilder()
            {
                Expiration = TimeSpan.FromMinutes(configuration.GetValue<double>("RefreshTokenLifeTimeMinutes")),
                HttpOnly = true,
                SecurePolicy = CookieSecurePolicy.Always,
                IsEssential = true,
                SameSite = SameSiteMode.Strict
            }.Build(httpContext);
        }
    }
}
