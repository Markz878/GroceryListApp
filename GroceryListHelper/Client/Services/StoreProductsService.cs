using Blazored.LocalStorage;
using GroceryListHelper.Client.Authentication;
using GroceryListHelper.Client.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Services
{
    public class StoreProductsService
    {
        private readonly HttpClient client;
        private const string uri = "api/storeproducts";
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly ILocalStorageService localStorage;
        private const string storeProductsKey = "storeProducts";

        public StoreProductsService(IHttpClientFactory clientFactory, AuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorage)
        {
            client = clientFactory.CreateClient("ProtectedClient");
            this.authenticationStateProvider = authenticationStateProvider;
            this.localStorage = localStorage;
        }

        public async Task<List<StoreProductUIModel>> GetStoreProducts()
        {
            return (await authenticationStateProvider.IsUserAuthenticated() ?
                 await client.GetFromJsonAsync<List<StoreProductUIModel>>(uri) :
                 await localStorage.GetItemAsync<List<StoreProductUIModel>>(storeProductsKey))
                 ?? new List<StoreProductUIModel>();
        }

        public async Task SaveStoreProduct(StoreProductUIModel product)
        {
            if (await authenticationStateProvider.IsUserAuthenticated())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(uri, product);
                if (response.IsSuccessStatusCode)
                {
                    string id = await response.Content.ReadAsStringAsync();
                    product.Id = int.Parse(id);
                }
                else
                {
                    throw new InvalidOperationException("Could not add product");
                }
            }
            else
            {
                List<StoreProductUIModel> products = await localStorage.GetItemAsync<List<StoreProductUIModel>>(storeProductsKey) ?? new List<StoreProductUIModel>();
                products.Add(product);
                await localStorage.SetItemAsync(storeProductsKey, products);
            }
        }

        public async Task ClearStoreProducts()
        {
            if (await authenticationStateProvider.IsUserAuthenticated())
            {
                HttpResponseMessage response = await client.DeleteAsync(uri);
            }
            else
            {
                await localStorage.RemoveItemAsync(storeProductsKey);
            }
        }

        public async Task UpdateStoreProductPrice(int id, double price)
        {
            if (await authenticationStateProvider.IsUserAuthenticated())
            {
                HttpResponseMessage response = await client.PatchAsync(uri + $"/{id}?price={price}", null);
            }
            else
            {
                List<StoreProductUIModel> products = await localStorage.GetItemAsync<List<StoreProductUIModel>>(storeProductsKey);
                StoreProductUIModel product = products.Find(x => x.Id == id);
                product.UnitPrice = price;
                await localStorage.SetItemAsync(storeProductsKey, products);
            }
        }
    }
}
