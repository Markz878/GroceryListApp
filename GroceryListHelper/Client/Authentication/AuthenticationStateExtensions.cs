using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace GroceryListHelper.Client.Authentication
{
    public static class AuthenticationStateExtensions
    {
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
}
