using GroceryListHelper.Client.Services;
using GroceryListHelper.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Pages
{
    [Authorize]
    public partial class Profile
    {
        [Inject] public ProfileService ProfileService { get; set; }
        [Inject] public AuthenticationStateProvider Authentication { get; set; }
        [Inject] public IJSRuntime JS { get; set; }
        public string Message { get; set; }

        private ChangePasswordRequest changePasswordRequest = new();
        private DeleteProfileRequest deleteProfileRequest = new();
        private string passwordMessage = string.Empty;
        private string deleteMessage = string.Empty;
        private string downloadMessage = string.Empty;
        private bool isBusy;

        private async Task ChangePassword()
        {
            try
            {
                isBusy = true;
                passwordMessage = await ProfileService.ChangePassword(changePasswordRequest);
                if (string.IsNullOrEmpty(passwordMessage))
                {
                    Message = "Password changed succesfully";
                    changePasswordRequest = new ChangePasswordRequest();
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
                    Message = "Profile deleted succesfully";
                    deleteProfileRequest = new DeleteProfileRequest();
                    await ProfileService.LogOut();
                }
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

        private async Task DownloadPersonalInfo()
        {
            try
            {
                isBusy = true;
                UserModel user = await ProfileService.DownloadPersonalData();
                if (user != null)
                {
                    await JS.InvokeVoidAsync("downloadObjectAsJson", new { user.Email }, "personaldata");
                }
                else
                {
                    downloadMessage = "Could not find your personal data";
                }
            }
            catch (Exception ex)
            {
                downloadMessage = ex.Message;
            }
            finally
            {
                isBusy = false;
            }
        }

    }
}
