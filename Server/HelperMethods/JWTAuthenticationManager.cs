﻿using GroceryListHelper.DataAccess.HelperMethods;
using GroceryListHelper.DataAccess.Models;
using GroceryListHelper.DataAccess.Repositories;
using GroceryListHelper.Shared.Models.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GroceryListHelper.Server.HelperMethods
{
    public class JWTAuthenticationManager : IJWTAuthenticationManager
    {
        private readonly IUserRepository userRepository;
        private readonly TokenValidationParametersFactory tokenValidationParametersFactory;
        private readonly ILogger<JWTAuthenticationManager> logger;
        private readonly IConfiguration configuration;
        private const string AccessTokenKey = "AccessTokenKey";
        private const string RefreshTokenKey = "RefreshTokenKey";

        public JWTAuthenticationManager(IConfiguration configuration,
                                        IUserRepository userRepository,
                                        TokenValidationParametersFactory tokenValidationParametersFactory,
                                        ILogger<JWTAuthenticationManager> logger)
        {
            this.userRepository = userRepository;
            this.tokenValidationParametersFactory = tokenValidationParametersFactory;
            this.logger = logger;
            this.configuration = configuration;
        }

        /// <summary>
        /// Registers a new user. Checks first if there is already a user with the given email.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>Returns an error message if email is already in use, otherwise null.</returns>
        public async Task<(AuthenticationResponseModel response, string refreshToken)> Register(string email, string password)
        {
            AuthenticationResponseModel response = new();
            if (await userRepository.GetUserFromEmail(email) != null)
            {
                response.ErrorMessage = "Email is already in use.";
                return (response, "");
            }
            UserDbModel user = await userRepository.AddUser(email, password);
            if (user == null)
            {
                response.ErrorMessage = "Error creating user.";
                return (response, "");
            }
            response.AccessToken = GenerateAccessToken(user);
            string refreshToken = GenerateRefreshToken(user);
            await userRepository.UpdateRefreshToken(user.Id, refreshToken);
            return (response, refreshToken);
        }

        public async Task<(AuthenticationResponseModel response, string refreshToken)> Login(string email, string password)
        {
            AuthenticationResponseModel response = new();
            UserDbModel user = await userRepository.GetUserFromEmail(email);
            if (user == null)
            {
                response.ErrorMessage = "User not found.";
                return (response, "");
            }
            if (!PasswordHelper.CheckPassword(user, password))
            {
                response.ErrorMessage = "Invalid password.";
                return (response, "");
            }

            response.AccessToken = GenerateAccessToken(user);
            string refreshToken = GenerateRefreshToken(user);
            await userRepository.UpdateRefreshToken(user.Id, refreshToken);
            return (response, refreshToken);
        }

        /// <summary>
        /// Creates a refresh JWT based on the previous refresh token claims.
        /// Checks that the previous refresh token has not expired and is otherwise valid.
        /// </summary>
        /// <param name="refreshToken">Previous refresh token</param>
        public async Task<(AuthenticationResponseModel response, string newRefreshToken)> RefreshTokens(string refreshToken)
        {
            AuthenticationResponseModel response = new();
            if (ValidateToken(RefreshTokenKey, refreshToken, out ClaimsPrincipal claimsPrincipal))
            {
                UserDbModel user = await userRepository.GetUserFromId(claimsPrincipal.GetUserId());
                if (user == null)
                {
                    response.ErrorMessage = "User not found.";
                    return (response, null);
                }
                if (user.RefreshToken != refreshToken)
                {
                    response.ErrorMessage = "Invalid refresh token.";
                    return (response, null);
                }
                response.AccessToken = GenerateAccessToken(user);
                string newRefreshToken = GenerateRefreshToken(user);
                await userRepository.UpdateRefreshToken(user.Id, newRefreshToken);
                return (response, newRefreshToken);
            }
            response.ErrorMessage = "Invalid refresh token";
            return (response, null);
        }

        private string GenerateAccessToken(UserModel user)
        {
            return GenerateToken(AccessTokenKey, TimeSpan.FromMinutes(configuration.GetValue<double>("AccessTokenLifeTimeMinutes")), user);
        }

        private string GenerateRefreshToken(UserModel user)
        {
            return GenerateToken(RefreshTokenKey, TimeSpan.FromMinutes(configuration.GetValue<double>("RefreshTokenLifeTimeMinutes")), user);
        }

        private string GenerateToken(string key, TimeSpan duration, UserModel user)
        {
            if (string.IsNullOrEmpty(key) || user == null)
            {
                throw new ArgumentNullException(nameof(user), "Error generating token.");
            }
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] keyBytes = Encoding.ASCII.GetBytes(configuration[key]);
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new Claim[] { new Claim("id", user.Id.ToString()), new Claim(ClaimTypes.Email, user.Email) }),
                Expires = DateTime.UtcNow + duration,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes),
                    SecurityAlgorithms.HmacSha512Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool ValidateToken(string key, string token, out ClaimsPrincipal claimsPrincipal)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            try
            {
                claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParametersFactory.CreateParameters(key), out SecurityToken jwt);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Token validation failed.");
                if (tokenHandler.CanReadToken(token))
                {
                    claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(tokenHandler.ReadJwtToken(token).Claims));
                }
                else
                {
                    claimsPrincipal = null;
                }
                return false;
            }
        }
    }
}