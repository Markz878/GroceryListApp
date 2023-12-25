using Microsoft.JSInterop;

namespace GroceryListHelper.Client.Authentication;

public class AntiforgeryTokenHandler(IJSRuntime js) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string token = await js.InvokeAsync<string>("getToken");
        request.Headers.TryAddWithoutValidation("RequestVerificationToken", token);
        return await base.SendAsync(request, cancellationToken);
    }
}
