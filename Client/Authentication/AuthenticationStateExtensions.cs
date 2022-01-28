using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Authentication;

public static class AuthenticationStateExtensions
{
    public static async Task<bool> IsUserAuthenticated(this AuthenticationStateProvider authenticationStateProvider)
    {
        AuthenticationState authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
        return authenticationState.User?.Identity?.IsAuthenticated == true;
    }

    public static bool AccessTokenStillValid(this string accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            return false;
        }
        IEnumerable<Claim> claims = accessToken.ParseTokenClaims();
        string expClaimValue = claims.First(x => x.Type == "exp").Value;
        DateTime expiryTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaimValue)).DateTime;
        DateTime utcNow = DateTime.UtcNow;
        bool isValid = expiryTime > utcNow;
        return isValid;
    }

    internal static IEnumerable<Claim> ParseTokenClaims(this string value)
    {
        string payload = value.Split('.')[1];
        byte[] payloadBytes = Parse64WithoutPadding(payload);
        Dictionary<string, object> claimsDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(payloadBytes);
        return claimsDictionary.Select(x => new Claim(x.Key, x.Value.ToString()));
    }

    private static byte[] Parse64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 0:
                break;
            case 2: // Two pad chars
                base64 += "==";
                break;
            case 3: // One pad char
                base64 += "=";
                break;
            default:
                throw new ArgumentException("Input was not a valid base 64 string");
        }
        return Convert.FromBase64String(base64);
    }
}
