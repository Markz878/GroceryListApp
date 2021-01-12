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

        private readonly UserCredentialsModel user = new UserCredentialsModel();
        private string message = string.Empty;
        private bool isBusy;

        private async Task LoginMethod()
        {
            try
            {
                isBusy = true;
                await AuthenticationService.Login(user);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            finally
            {
                isBusy = false;
            }
        }
    }
}
