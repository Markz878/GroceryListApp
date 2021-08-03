using GroceryListHelper.Client.HelperMethods;
using GroceryListHelper.Client.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;

namespace GroceryListHelper.Client.ViewModels
{
    public class IndexViewModel : BaseViewModel
    {
        public List<CartProductUIModel> CartProducts { get => cartProducts; set => SetProperty(ref cartProducts, value); }
        private List<CartProductUIModel> cartProducts = new();
        public List<StoreProductUIModel> StoreProducts { get => storeProducts; set => SetProperty(ref storeProducts, value); }
        private List<StoreProductUIModel> storeProducts = new();
        public ShareModeType ShareMode { get => shareMode; set => SetProperty(ref shareMode, value); }
        private ShareModeType shareMode;
        public List<string> AllowedUsers { get => allowedUsers; set => SetProperty(ref allowedUsers, value); }
        private List<string> allowedUsers = new();
        public string CartHostEmail { get => cartHostEmail; set => SetProperty(ref cartHostEmail, value); }
        private string cartHostEmail;
        public string AllowEmail { get => allowEmail; set => SetProperty(ref allowEmail, value); }
        private string allowEmail;
        public string ShareCartInfo { get => shareCartInfo; set => SetProperty(ref shareCartInfo, value); }
        private string shareCartInfo;
        public HubConnection CartHub { get; set; }
        public bool IsPolling { get => isPolling; set => SetProperty(ref isPolling, value); }
        private bool isPolling;
    }
}
