using GroceryListHelper.Client.Services;
using GroceryListHelper.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Pages
{
    public partial class Profile
    {
        [Inject] public ProfileService ProfileService { get; set; }
        [Inject] public AuthenticationStateProvider Authentication { get; set; }

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
                string response = await ProfileService.ChangePassword(changePasswordRequest);
                passwordMessage = response ?? "Password changed";
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
                string response = await ProfileService.Delete(deleteProfileRequest);
                deleteMessage = response ?? "Profile deleted";
                (Authentication as CustomAuthenticationStateProvider).Logout();
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
