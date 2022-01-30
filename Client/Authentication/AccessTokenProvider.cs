using GroceryListHelper.Shared.Models.Authentication;
using System.Net.Http.Json;
using System.Text.Json;

namespace GroceryListHelper.Client.Authentication;

public class AccessTokenProvider : IAccessTokenProvider
{
    private readonly HttpClient client;
    private string accessToken;
    private readonly SemaphoreSlim semaphore = new(1, 1);
    private DateTimeOffset previousRefreshTime;

    public AccessTokenProvider(IHttpClientFactory httpClientFactory)
    {
        client = httpClientFactory.CreateClient("AnonymousClient");
    }

    public async ValueTask<string> RequestAccessToken()
    {
        await semaphore.WaitAsync();
        if (previousRefreshTime < DateTimeOffset.UtcNow.AddSeconds(-1) && (string.IsNullOrEmpty(accessToken) || !accessToken.AccessTokenStillValid()))
        {
            accessToken = await TryToRefreshToken();
            previousRefreshTime = DateTimeOffset.UtcNow;
        }
        semaphore.Release();
        return accessToken;
    }

    public async ValueTask<string> TryToRefreshToken()
    {
        HttpResponseMessage response = await client.GetAsync("api/authentication/refresh");
        if (response.IsSuccessStatusCode)
        {
            AuthenticationResponseModel loginResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponseModel>(new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            await SaveToken(loginResponse.AccessToken);
            return loginResponse.AccessToken;
        }
        else
        {
            return null;
        }
    }

    public ValueTask SaveToken(string accessToken)
    {
        this.accessToken = accessToken;
        return ValueTask.CompletedTask;
    }

    public ValueTask RemoveToken()
    {
        accessToken = string.Empty;
        return ValueTask.CompletedTask;
    }
}
