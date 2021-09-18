using GroceryListHelper.Client.HelperMethods;
using GroceryListHelper.Client.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace GroceryListHelper.Client.ViewModels
{
    public class IndexViewModel : BaseViewModel, IDisposable
    {
        public ObservableCollection<CartProductUIModel> CartProducts { get; } = new();
        public ObservableCollection<StoreProductUIModel> StoreProducts { get; } = new();
        public ObservableCollection<string> AllowedUsers { get; } = new();
        public ShareModeType ShareMode { get => shareMode; set => SetProperty(ref shareMode, value); }
        private ShareModeType shareMode;
        public string CartHostEmail { get => cartHostEmail; set => SetProperty(ref cartHostEmail, value); }
        private string cartHostEmail;
        public string AllowEmail { get => allowEmail; set => SetProperty(ref allowEmail, value); }
        private string allowEmail;
        public string ShareCartInfo { get => shareCartInfo; set => SetProperty(ref shareCartInfo, value); }
        private string shareCartInfo;
        public HubConnection CartHub { get; set; }
        public bool IsPolling { get => isPolling; set => SetProperty(ref isPolling, value); }
        private bool isPolling;
        public bool ShowOnlyUncollected { get => showOnlyUncollected; set => SetProperty(ref showOnlyUncollected, value); }
        private bool showOnlyUncollected;

        public IndexViewModel()
        {
            CartProducts.CollectionChanged += CollectionChanged;
            StoreProducts.CollectionChanged += CollectionChanged;
            AllowedUsers.CollectionChanged += CollectionChanged;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged();
        }

        public void Dispose()
        {
            CartProducts.CollectionChanged -= CollectionChanged;
            StoreProducts.CollectionChanged -= CollectionChanged;
            AllowedUsers.CollectionChanged -= CollectionChanged;
        }
    }
}
