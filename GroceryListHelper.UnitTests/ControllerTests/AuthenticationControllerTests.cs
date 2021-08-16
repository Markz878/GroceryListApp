using GroceryListHelper.Server.Controllers;
using GroceryListHelper.Server.HelperMethods;
using GroceryListHelper.Shared;
using GroceryListHelper.UnitTests.HelperModels;
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
            RegisterRequestModel registerRequest = new() { Email = "ms@gmail.com", Password = "passu123", ConfirmPassword = "passu123" };
            AuthenticationResponseModel expectedResponse = new() { AccessToken = "access_token" };
            jwtAuthentication.Register(registerRequest.Email, registerRequest.Password).Returns(Task.FromResult((expectedResponse, "refresh_token")));
            // Act
            ActionResult<AuthenticationResponseModel> actionResult = await authenticationController.Register(registerRequest);
            OkObjectResult objectResult = actionResult.Result as OkObjectResult;
            // Assert
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(expectedResponse.AccessToken, (objectResult.Value as AuthenticationResponseModel).AccessToken);
            Assert.Equal(expectedResponse.ErrorMessage, (objectResult.Value as AuthenticationResponseModel).ErrorMessage);
        }

        [Fact]
        public async Task AuthenticationController_Register_GivenBadRegisterRequestModel_ReturnsBadRequestWithErrorResponse()
        {
            // Arrange
            RegisterRequestModel registerRequest = new() { Email = "ms@gmail.com", Password = "p", ConfirmPassword = "pa" };
            AuthenticationResponseModel expectedResponse = new() { ErrorMessage = "Invalid password" };
            jwtAuthentication.Register(registerRequest.Email, registerRequest.Password).Returns(Task.FromResult((expectedResponse, "")));
            // Act
            ActionResult<AuthenticationResponseModel> actionResult = await authenticationController.Register(registerRequest);
            BadRequestObjectResult objectResult = actionResult.Result as BadRequestObjectResult;
            // Assert
            Assert.Equal(400, objectResult.StatusCode);
            Assert.Equal(expectedResponse.AccessToken, (objectResult.Value as AuthenticationResponseModel).AccessToken);
            Assert.Equal(expectedResponse.ErrorMessage, (objectResult.Value as AuthenticationResponseModel).ErrorMessage);
        }

        [Fact]
        public async Task AuthenticationController_Login_GivenValidLoginRequestModel_ReturnsOkWithValidResponse()
        {
            // Arrange
            UserCredentialsModel userCredentials = new() { Email = "ms@gmail.com", Password = "passu123" };
            AuthenticationResponseModel expectedResponse = new() { AccessToken = "access_token" };
            jwtAuthentication.Login(userCredentials.Email, userCredentials.Password).Returns(Task.FromResult((expectedResponse, "refresh_token")));
            // Act
            ActionResult<AuthenticationResponseModel> actionResult = await authenticationController.Login(userCredentials);
            OkObjectResult objectResult = actionResult.Result as OkObjectResult;
            // Assert
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(expectedResponse.AccessToken, (objectResult.Value as AuthenticationResponseModel).AccessToken);
            Assert.Equal(expectedResponse.ErrorMessage, (objectResult.Value as AuthenticationResponseModel).ErrorMessage);
        }

        [Fact]
        public async Task AuthenticationController_Login_GivenBadLoginRequestModel_ReturnsBadRequestWithErrorResponse()
        {
            // Arrange
            UserCredentialsModel userCredentials = new() { Email = "ms@gmail.com", Password = "p" };
            AuthenticationResponseModel expectedResponse = new() { ErrorMessage = "Invalid password" };
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
            AuthenticationResponseModel expectedResponse = new() { AccessToken = "access_token" };
            jwtAuthentication.RefreshTokens("refresh_token").Returns(Task.FromResult((expectedResponse, "refresh_token")));
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
            jwtAuthentication.RefreshTokens("refresh_token").Returns(Task.FromResult((expectedResponse, "refresh_token")));
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
