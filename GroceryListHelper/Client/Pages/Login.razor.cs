using System;
using System.Threading.Tasks;
using GroceryListHelper.Client.Services;
using GroceryListHelper.Shared;
using Microsoft.AspNetCore.Components;

namespace GroceryListHelper.Client.Pages
{
    public partial class Login
    {
        [Inject] public AuthenticationService AuthenticationService { get; set; }
        [Inject] public NavigationManager Navigation { get; set; } 

        private readonly UserCredentialsModel user = new();
        public string Message { get; set; } = string.Empty;
        private bool isBusy;

        private async Task LoginMethod()
        {
            try
            {
                isBusy = true;
                string error = await AuthenticationService.Login(user);
                if (!string.IsNullOrEmpty(error))
                {
                    Message = error;
                }
                else
                {
                    Navigation.NavigateTo("/", true);
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            finally
            {
                isBusy = false;
            }
        }
    }
}
