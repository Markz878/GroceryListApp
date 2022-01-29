using GroceryListHelper.Client.Authentication;
using GroceryListHelper.Shared.Models.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Services;

public class ProfileService
{
    private const string uri = "api/profile";
    private readonly HttpClient client;
    private readonly NavigationManager navigation;
    private readonly IAccessTokenProvider accessTokenProvider;
    private readonly AuthenticationStateProvider authenticationStateProvider;

    public ProfileService(IHttpClientFactory clientFactory, IAccessTokenProvider accessTokenProvider, AuthenticationStateProvider authenticationStateProvider, NavigationManager navigation)
    {
        client = clientFactory.CreateClient("ProtectedClient");
        this.accessTokenProvider = accessTokenProvider;
        this.authenticationStateProvider = authenticationStateProvider;
        this.navigation = navigation;
    }

    public async Task LogOut()
    {
        await client.GetAsync(uri + "/logout");
        await accessTokenProvider.RemoveToken();
        await authenticationStateProvider.GetAuthenticationStateAsync();
        navigation.NavigateTo("/", true);
    }

    public async Task<string> ChangeEmail(ChangeEmailRequest changeEmailRequest)
    {
        HttpResponseMessage response = await client.PatchAsync(uri + "/changeemail", JsonContent.Create(changeEmailRequest));
        if (response.IsSuccessStatusCode)
        {
            return null;
        }
        else
        {
            return await response.Content.ReadAsStringAsync();
        }
    }

    public async Task<string> ChangePassword(ChangePasswordRequest changePasswordRequest)
    {
        HttpResponseMessage response = await client.PatchAsync(uri + "/changepassword", JsonContent.Create(changePasswordRequest));
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
        HttpRequestMessage request = new(HttpMethod.Delete, uri + "/delete");
        request.Content = JsonContent.Create(user);
        HttpResponseMessage response = await client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return null;
        }
        else
        {
            return await response.Content.ReadAsStringAsync();
        }
    }

    public Task<UserModel> GetUserInfo()
    {
        return client.GetFromJsonAsync<UserModel>(uri + "/getuserinfo");
    }
}
