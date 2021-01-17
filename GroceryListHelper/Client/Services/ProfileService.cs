using GroceryListHelper.Shared;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Services
{
    public class ProfileService
    {
        private const string uri = "api/profile";
        private readonly HttpClient client;

        public ProfileService(IHttpClientFactory clientFactory)
        {
            client = clientFactory.CreateClient("ProtectedClient");
        }

        internal async Task<string> ChangePassword(ChangePasswordRequest changePasswordRequest)
        {
            var response = await client.PatchAsync(uri, JsonContent.Create(changePasswordRequest));
            if (response.IsSuccessStatusCode)
            {
                return null;
            }
            else
            {
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> Delete(DeleteProfileRequest user)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, uri);
            request.Content = JsonContent.Create(user);
            Console.WriteLine(uri);
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return null;
            }
            else
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
