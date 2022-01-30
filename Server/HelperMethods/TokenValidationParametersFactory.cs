using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GroceryListHelper.Server.HelperMethods;

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
            ClockSkew = TimeSpan.FromSeconds(5),
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha512 },
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    }
}
