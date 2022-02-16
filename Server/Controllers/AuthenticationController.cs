using FluentValidation;
using GroceryListHelper.Server.HelperMethods;
using GroceryListHelper.Server.Validators;
using GroceryListHelper.Shared.Models.Authentication;
using GroceryListHelper.Shared.Models.BaseModels;
using Microsoft.AspNetCore.Mvc;

namespace GroceryListHelper.Server.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly IJWTAuthenticationManager authenticationManager;
    private readonly IValidator<RegisterRequestModel> registerValidator;
    private readonly IValidator<UserCredentialsModel> loginValidator;

    public AuthenticationController(IConfiguration configuration, IJWTAuthenticationManager authenticationManager, IValidator<RegisterRequestModel> registerValidator, IValidator<UserCredentialsModel> loginValidator)
    {
        this.configuration = configuration;
        this.authenticationManager = authenticationManager;
        this.registerValidator = registerValidator;
        this.loginValidator = loginValidator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticationResponseModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthenticationResponseModel))]
    public async Task<ActionResult<AuthenticationResponseModel>> Register([FromBody] RegisterRequestModel user)
    {
        if (registerValidator.FindsError(user, out string error))
        {
            return BadRequest(new AuthenticationResponseModel() { ErrorMessage = error });
        }
        (AuthenticationResponseModel response, string refreshToken) = await authenticationManager.Register(user.Email, user.Password);
        if (string.IsNullOrEmpty(response?.ErrorMessage) && !string.IsNullOrEmpty(response?.AccessToken) && !string.IsNullOrEmpty(refreshToken))
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
        if (loginValidator.FindsError(user, out string error))
        {
            return BadRequest(new AuthenticationResponseModel() { ErrorMessage = error });
        }
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
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
            return NoContent();
        }
        return NoContent();
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
