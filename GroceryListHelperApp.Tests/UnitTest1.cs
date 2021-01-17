using System;
using System.Diagnostics;
using Xunit;

namespace GroceryListHelperApp.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            for (int i = 65; i <= 90; i++)
            {
                char result = (char)i;
            }
        }
    }
}
