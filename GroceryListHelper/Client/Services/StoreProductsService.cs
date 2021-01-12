using GroceryListHelper.Client.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static GroceryListHelper.Client.HelperMethods.HelperMethods;

namespace GroceryListHelper.Client.Services
{
    public class StoreProductsService
    {
        private readonly HttpClient client;
        private const string uri = "api/storeproducts";

        public StoreProductsService(IHttpClientFactory clientFactory)
        {
            client = clientFactory.CreateClient("ProtectedClient");
        }

        public Task<List<StoreProductUIModel>> GetCartProducts()
        {
            try
            {
                return client.GetFromJsonAsync<List<StoreProductUIModel>>(uri);
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
                return Task.FromResult(new List<StoreProductUIModel>());
            }
        }

        public async Task SaveStoreProduct(StoreProductUIModel product)
        {
            try
            {
                var response = await client.PostAsJsonAsync(uri, product);
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
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }

        public Task ClearStoreProducts()
        {
            return CheckAccessToken(client.DeleteAsync(uri));
        }

        public Task UpdateStoreProductPrice(int id, double price)
        {
            return CheckAccessToken(client.PatchAsync(uri + $"/{id}?price={price}", null));
        }
    }
}
