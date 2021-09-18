using GroceryListHelper.Client.Services;
using GroceryListHelper.Client.ViewModels;
using GroceryListHelper.Shared.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Pages
{
    [Authorize]
    public class ProfileBase : ComponentBase
    {
        [Inject] public ProfileService ProfileService { get; set; }
        [Inject] public IJSRuntime JS { get; set; }
        [Inject] public ModalViewModel ModalViewModel { get; set; }

        public ChangeEmailRequest changeEmailRequest = new();
        public ChangePasswordRequest changePasswordRequest = new();
        public DeleteProfileRequest deleteProfileRequest = new();
        public string emailMessage = string.Empty;
        public string passwordMessage = string.Empty;
        public string deleteMessage = string.Empty;
        public string downloadMessage = string.Empty;
        public bool isBusy;

        public async Task ChangeEmail()
        {
            try
            {
                isBusy = true;
                emailMessage = await ProfileService.ChangeEmail(changeEmailRequest);
                if (string.IsNullOrEmpty(emailMessage))
                {
                    ModalViewModel.Message = "Email changed succesfully, please login with new email.";
                    changeEmailRequest = new();
                    await Task.Delay(2000);
                    await ProfileService.LogOut();
                }
            }
            catch (Exception ex)
            {
                emailMessage = ex.Message;
            }
            finally
            {
                isBusy = false;
            }
        }

        public async Task ChangePassword()
        {
            try
            {
                isBusy = true;
                passwordMessage = await ProfileService.ChangePassword(changePasswordRequest);
                if (string.IsNullOrEmpty(passwordMessage))
                {
                    ModalViewModel.Message = "Password changed succesfully, please login with new password.";
                    changePasswordRequest = new ChangePasswordRequest();
                    await Task.Delay(2000);
                    await ProfileService.LogOut();
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

        public async Task DeleteProfile()
        {
            try
            {
                isBusy = true;
                deleteMessage = await ProfileService.Delete(deleteProfileRequest);
                if (string.IsNullOrEmpty(deleteMessage))
                {
                    ModalViewModel.Message = "Profile deleted succesfully.";
                    deleteProfileRequest = new DeleteProfileRequest();
                    await Task.Delay(2000);
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

        public async Task DownloadPersonalInfo()
        {
            try
            {
                isBusy = true;
                UserModel user = await ProfileService.GetUserInfo();
                if (user != null)
                {
                    await JS.InvokeVoidAsync("downloadObjectAsJson", new { user.Email }, "personaldata");
                }
                else
                {
                    downloadMessage = "Could not find your personal data.";
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
