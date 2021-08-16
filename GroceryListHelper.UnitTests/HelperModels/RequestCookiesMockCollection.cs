using GroceryListHelper.Shared;
using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GroceryListHelper.UnitTests.HelperModels
{
    internal class RequestCookiesMockCollection : IRequestCookieCollection
    {
        public string this[string key] { get => cookieValues[key]; set => cookieValues[key] = value; }


        public RequestCookiesMockCollection()
        {
        }

        public RequestCookiesMockCollection(IDictionary<string, string> cookieValues)
        {
            foreach (KeyValuePair<string, string> item in cookieValues)
            {
                cookieValues.TryAdd(item.Key, item.Value);
            }
        }

        public int Count => cookieValues.Count;
        public ICollection<string> Keys => cookieValues.Keys;

        private readonly IDictionary<string, string> cookieValues = new Dictionary<string, string>();

        public bool ContainsKey(string key)
        {
            return cookieValues.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return cookieValues.GetEnumerator();
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
        {
            return cookieValues.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return cookieValues.GetEnumerator();
        }
    }
}
