using GroceryListHelper.Client.Services;
using GroceryListHelper.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Pages
{
    public partial class Profile
    {
        [Inject] public ProfileService ProfileService { get; set; }
        [Inject] public AuthenticationStateProvider Authentication { get; set; }
        [Inject] public IJSRuntime JS { get; set; }

        private string email = string.Empty;
        private readonly ChangePasswordRequest changePasswordRequest = new ChangePasswordRequest();
        private readonly DeleteProfileRequest deleteProfileRequest = new DeleteProfileRequest();
        private string passwordMessage = string.Empty;
        private string deleteMessage = string.Empty;
        private bool isBusy;

        protected override async Task OnInitializedAsync()
        {
            var authState = await Authentication.GetAuthenticationStateAsync();
            email = authState.User.FindFirst("email").Value;
        }

        private async Task ChangePassword()
        {
            try
            {
                isBusy = true;
                passwordMessage = await ProfileService.ChangePassword(changePasswordRequest);
                if (string.IsNullOrEmpty(passwordMessage))
                {
                    await JS.InvokeVoidAsync("alert", "Password changed succesfully");
                }
            }
            catch (Exception ex)
            {
                passwordMessage = ex.Message;
            }
            finally
            {
                isBusy = false;
            }
        }

        private async Task DeleteProfile()
        {
            try
            {
                isBusy = true;
                deleteMessage = await ProfileService.Delete(deleteProfileRequest);
                if (string.IsNullOrEmpty(deleteMessage))
                {
                    await JS.InvokeVoidAsync("alert", "Profile deleted succesfully");
                }
                (Authentication as CustomAuthenticationStateProvider)?.NotifyLogOut();
            }
            catch (Exception ex)
            {
                deleteMessage = ex.Message;
            }
            finally
            {
                isBusy = false;
            }
        }
    }
}
