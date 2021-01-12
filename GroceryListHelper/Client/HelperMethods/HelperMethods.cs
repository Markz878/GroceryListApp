using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Globalization;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.HelperMethods
{
    public static class HelperMethods
    {
        public static bool TryParseDouble(string s, out double result)
        {
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out result))
            {
                return true;
            }
            else if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task CheckAccessToken<T>(Task<T> method)
        {
            try
            {
                await method;
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }
    }
}
