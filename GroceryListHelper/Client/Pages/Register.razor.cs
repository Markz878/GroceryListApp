using GroceryListHelper.Client.Services;
using GroceryListHelper.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;


namespace GroceryListHelper.Client.Pages
{
    public partial class Register
    {
        [Inject] public AuthenticationService AuthenticationService { get; set; }

        private readonly RegisterRequestModel registerRequest = new();
        public string Message { get; set; } = string.Empty;
        private bool isBusy;

        private async Task RegisterMethod()
        {
            try
            {
                isBusy = true;
                await AuthenticationService.Register(registerRequest);
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
