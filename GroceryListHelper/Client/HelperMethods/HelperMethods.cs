using System;
using System.Globalization;

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

        public static string CreateRandomRoomName()
        {
            Random rng = new Random();
            char[] result = new char[6];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (char)rng.Next(65, 91);
            }
            return new string(result);
        }
    }
}
