using GroceryListHelper.Client.Authentication;
using GroceryListHelper.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Services
{
    public class AuthenticationService
    {
        private const string uri = "api/authentication";
        private readonly HttpClient client;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly JsonSerializerOptions jsonOptions;
        private readonly IAccessTokenProvider accessTokenHandler;

        public AuthenticationService(IHttpClientFactory clientFactory, IAccessTokenProvider accessTokenHandler, AuthenticationStateProvider authenticationStateProvider, JsonSerializerOptions jsonOptions)
        {
            client = clientFactory.CreateClient("AnonymousClient");
            this.authenticationStateProvider = authenticationStateProvider;
            this.jsonOptions = jsonOptions;
            this.accessTokenHandler = accessTokenHandler;
        }

        public async Task<string> Login(UserCredentialsModel user)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(uri + "/login", user);
            return await SaveToken(response);
        }

        public async Task<string> Register(RegisterRequestModel request)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(uri + "/register", request);
            return await SaveToken(response);
        }

        private async Task<string> SaveToken(HttpResponseMessage response)
        {
            AuthenticationResponseModel tokenResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponseModel>(jsonOptions);
            if (response.IsSuccessStatusCode)
            {
                await accessTokenHandler.SaveToken(tokenResponse.AccessToken);
                await authenticationStateProvider.GetAuthenticationStateAsync();
                return null;
            }
            else
            {
                return tokenResponse.ErrorMessage;
            }
        }
    }
}