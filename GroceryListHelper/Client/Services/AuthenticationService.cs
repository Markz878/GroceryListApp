using GroceryListHelper.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
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
        private readonly NavigationManager navigation;
        private readonly CustomAccessTokenProvider accessTokenProvider;
        private readonly AuthenticationStateProvider authenticationStateProvider;

        public AuthenticationService(IHttpClientFactory clientFactory, NavigationManager navigation, IAccessTokenProvider accessTokenProvider, AuthenticationStateProvider authenticationStateProvider)
        {
            client = clientFactory.CreateClient("AnonymousClient");
            this.navigation = navigation;
            this.accessTokenProvider = accessTokenProvider as CustomAccessTokenProvider;
            this.authenticationStateProvider = authenticationStateProvider;
        }

        public async Task Login(UserCredentialsModel user)
        {
            var response = await client.PostAsJsonAsync(uri + "/login", user);
            await SaveTokenAndGoToIndex(response);
        }

        public async Task Register(RegisterRequestModel request)
        {
            var response = await client.PostAsJsonAsync(uri + "/register", request);
            await SaveTokenAndGoToIndex(response);
        }

        public async Task Refresh()
        {
            var response = await client.GetAsync(uri + "/refresh");
            await SaveTokenAndGoToIndex(response);
        }

        private async Task SaveTokenAndGoToIndex(HttpResponseMessage response)
        {
            LoginResponseModel tokens = await response.Content.ReadFromJsonAsync<LoginResponseModel>(new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            if (response.IsSuccessStatusCode)
            {
                accessTokenProvider.SaveAccessToken(tokens.AccessToken);
                await authenticationStateProvider.GetAuthenticationStateAsync();
                navigation.NavigateTo("/");
            }
            else
            {
                throw new ArgumentException(tokens.Message);
            }
        }
    }
}
