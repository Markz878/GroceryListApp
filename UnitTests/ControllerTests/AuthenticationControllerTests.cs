using Bogus;
using GroceryListHelper.Server.Controllers;
using GroceryListHelper.Server.HelperMethods;
using GroceryListHelper.Shared.Models.Authentication;
using GroceryListHelper.Shared.Models.BaseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using NSubstitute;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace GroceryListHelper.UnitTests.ControllerTests
{
    public class AuthenticationControllerTests
    {
        private readonly AuthenticationController authenticationController;
        private readonly IConfiguration configuration;
        private readonly IJWTAuthenticationManager jwtAuthentication;

        public AuthenticationControllerTests()
        {
            Dictionary<string, string> configurationValues = new()
            {
                { "RefreshTokenLifeTimeMinutes", "500" }
            };
            configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationValues).Build();
            jwtAuthentication = Substitute.For<IJWTAuthenticationManager>();
            authenticationController = new AuthenticationController(configuration, jwtAuthentication)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public async Task AuthenticationController_Register_GivenValidRegisterRequestModel_ReturnsOkWithValidResponse()
        {
            // Arrange
            RegisterRequestModel validRegisterRequest = new Faker<RegisterRequestModel>()
                .RuleFor(x => x.Email, f => f.Internet.Email())
                .RuleFor(x => x.Password, f => f.Random.Replace("????###"))
                .RuleFor(x => x.Password, (f, r) => r.Password)
                .Generate();
            AuthenticationResponseModel expectedResponse = new Faker<AuthenticationResponseModel>()
                .RuleFor(x => x.AccessToken, f => f.Random.String2(50))
                .Generate();
            string refreshToken = new Faker().Random.String2(50);
            jwtAuthentication.Register(validRegisterRequest.Email, validRegisterRequest.Password).Returns(Task.FromResult((expectedResponse, refreshToken)));
            // Act
            ActionResult<AuthenticationResponseModel> actionResult = await authenticationController.Register(validRegisterRequest);
            // Assert
            Assert.True(actionResult.Result is OkObjectResult);
            OkObjectResult objectResult = actionResult.Result as OkObjectResult;
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(expectedResponse.AccessToken, (objectResult.Value as AuthenticationResponseModel).AccessToken);
            Assert.Equal(expectedResponse.ErrorMessage, (objectResult.Value as AuthenticationResponseModel).ErrorMessage);
        }

        [Fact]
        public async Task AuthenticationController_Register_GivenBadRegisterRequestModel_ReturnsBadRequestWithErrorResponse()
        {
            // Arrange
            RegisterRequestModel invalidRegisterRequest = new Faker<RegisterRequestModel>()
                .RuleFor(x => x.Email, f => f.Internet.Email())
                .RuleFor(x => x.Password, f => f.Random.Replace("????###"))
                .RuleFor(x => x.Password, f => f.Random.Replace("??????###"))
                .Generate();
            AuthenticationResponseModel expectedResponse = new Faker<AuthenticationResponseModel>()
                .RuleFor(x => x.ErrorMessage, new Faker().Random.String2(10))
                .Generate();
            jwtAuthentication.Register(invalidRegisterRequest.Email, invalidRegisterRequest.Password).Returns(Task.FromResult((expectedResponse, "")));
            // Act
            ActionResult<AuthenticationResponseModel> actionResult = await authenticationController.Register(invalidRegisterRequest);
            // Assert
            Assert.True(actionResult.Result is BadRequestObjectResult);
            BadRequestObjectResult objectResult = actionResult.Result as BadRequestObjectResult;
            Assert.Equal(400, objectResult.StatusCode);
            Assert.Equal(expectedResponse.AccessToken, (objectResult.Value as AuthenticationResponseModel).AccessToken);
            Assert.Equal(expectedResponse.ErrorMessage, (objectResult.Value as AuthenticationResponseModel).ErrorMessage);
        }

        [Fact]
        public async Task AuthenticationController_Login_GivenValidLoginRequestModel_ReturnsOkWithValidResponse()
        {
            // Arrange
            UserCredentialsModel userCredentials = new Faker<UserCredentialsModel>()
                .RuleFor(x => x.Email, f => f.Internet.Email())
                .RuleFor(x => x.Password, f => f.Random.Replace("####!!!"))
                .Generate();

            AuthenticationResponseModel expectedResponse = new Faker<AuthenticationResponseModel>()
                .RuleFor(x => x.AccessToken, f => f.Random.String2(30));

            jwtAuthentication.Login(userCredentials.Email, userCredentials.Password).Returns(Task.FromResult((expectedResponse, new Faker().Random.String2(40))));
            // Act
            ActionResult<AuthenticationResponseModel> actionResult = await authenticationController.Login(userCredentials);
            // Assert
            Assert.True(actionResult.Result is OkObjectResult);
            OkObjectResult objectResult = actionResult.Result as OkObjectResult;
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(expectedResponse.AccessToken, (objectResult.Value as AuthenticationResponseModel).AccessToken);
            Assert.Equal(expectedResponse.ErrorMessage, (objectResult.Value as AuthenticationResponseModel).ErrorMessage);
        }

        [Fact]
        public async Task AuthenticationController_Login_GivenBadLoginRequestModel_ReturnsBadRequestWithErrorResponse()
        {
            // Arrange
            UserCredentialsModel userCredentials = new Faker<UserCredentialsModel>()
                .RuleFor(x => x.Email, f => f.Internet.Email())
                .RuleFor(x => x.Password, f => f.Random.String2(1, 10))
                .Generate();

            AuthenticationResponseModel expectedResponse = new Faker<AuthenticationResponseModel>()
                .RuleFor(x => x.ErrorMessage, f => f.Random.String2(30));

            jwtAuthentication.Login(userCredentials.Email, userCredentials.Password).Returns(Task.FromResult((expectedResponse, "")));
            // Act
            ActionResult<AuthenticationResponseModel> actionResult = await authenticationController.Login(userCredentials);
            BadRequestObjectResult objectResult = actionResult.Result as BadRequestObjectResult;
            // Assert
            Assert.Equal(400, objectResult.StatusCode);
            Assert.Equal(expectedResponse.AccessToken, (objectResult.Value as AuthenticationResponseModel).AccessToken);
            Assert.Equal(expectedResponse.ErrorMessage, (objectResult.Value as AuthenticationResponseModel).ErrorMessage);
        }

        [Fact]
        public async Task AuthenticationController_Refresh_UserHasValidRefreshtokenCookie_ReturnsOkWithValidResponse()
        {
            // Arrange
            AuthenticationResponseModel expectedResponse = new Faker<AuthenticationResponseModel>()
                .RuleFor(x => x.AccessToken, f => f.Random.String2(30));
            jwtAuthentication.RefreshTokens("refresh_token").Returns(Task.FromResult((expectedResponse, new Faker().Random.String2(40))));
            StringValues cookie = new(GlobalConstants.XRefreshToken + "=" + "refresh_token");
            authenticationController.ControllerContext.HttpContext.Request.Headers.Add(HeaderNames.Cookie, cookie);
            // Act
            ActionResult<AuthenticationResponseModel> actionResult = await authenticationController.Refresh();
            OkObjectResult objectResult = actionResult.Result as OkObjectResult;
            // Assert
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(expectedResponse.AccessToken, (objectResult.Value as AuthenticationResponseModel).AccessToken);
            Assert.Equal(expectedResponse.ErrorMessage, (objectResult.Value as AuthenticationResponseModel).ErrorMessage);
        }

        [Fact]
        public async Task AuthenticationController_Refresh_UserHasNoRefreshtokenCookie_ReturnsBadRequestWithResponse()
        {
            // Arrange
            AuthenticationResponseModel expectedResponse = new() { ErrorMessage = "No refresh token in cookies." };
            jwtAuthentication.RefreshTokens(new Faker().Random.String2(40)).Returns(Task.FromResult((expectedResponse, new Faker().Random.String2(40))));
            // Act
            ActionResult<AuthenticationResponseModel> actionResult = await authenticationController.Refresh();
            BadRequestObjectResult objectResult = actionResult.Result as BadRequestObjectResult;
            // Assert
            Assert.Equal(400, objectResult.StatusCode);
            Assert.Equal(expectedResponse.AccessToken, (objectResult.Value as AuthenticationResponseModel).AccessToken);
            Assert.Equal(expectedResponse.ErrorMessage, (objectResult.Value as AuthenticationResponseModel).ErrorMessage);
        }
    }
}
