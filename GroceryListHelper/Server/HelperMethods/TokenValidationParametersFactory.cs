using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace GroceryListHelper.Server.HelperMethods
{
    public class TokenValidationParametersFactory
    {
        private readonly IConfiguration configuration;

        public TokenValidationParametersFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public TokenValidationParameters CreateParameters(string key)
        {
            return new()
            {
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration[key])),
                ClockSkew = TimeSpan.Zero,
                ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha512 },
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = configuration["HostUrl"],
                ValidAudience = configuration["HostUrl"],
            };
        }
    }
}
