using Blazored.LocalStorage;
using GroceryListHelper.Client.Authentication;
using GroceryListHelper.Client.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GroceryListHelper.Client.Services
{
    public class CartProductsService
    {
        private readonly HttpClient client;
        private readonly ILocalStorageService localStorage;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private const string uri = "api/cartproducts";
        private const string cartProductsKey = "cartProducts";

        public CartProductsService(ILocalStorageService localStorage, IHttpClientFactory clientFactory, AuthenticationStateProvider authenticationStateProvider)
        {
            client = clientFactory.CreateClient("ProtectedClient");
            this.localStorage = localStorage;
            this.authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<List<CartProductUIModel>> GetCartProducts()
        {
            return (await authenticationStateProvider.IsUserAuthenticated() ?
                await client.GetFromJsonAsync<List<CartProductUIModel>>(uri) :
                await localStorage.GetItemAsync<List<CartProductUIModel>>(cartProductsKey))
                ?? new List<CartProductUIModel>();
        }

        public async Task SaveCartProduct(CartProductUIModel product)
        {
            if (await authenticationStateProvider.IsUserAuthenticated())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(uri, product);
                string content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    product.Id = int.Parse(content);
                }
            }
            else
            {
                List<CartProductUIModel> products = await localStorage.GetItemAsync<List<CartProductUIModel>>(cartProductsKey) ?? new List<CartProductUIModel>();
                product.Id = GetNextId(products);
                products.Add(product);
                await localStorage.SetItemAsync(cartProductsKey, products);
            }
        }

        private static int GetNextId(IEnumerable<CartProductUIModel> products)
        {
            int id = 0;
            while (true)
            {
                if (products.Any(x => x.Id == id))
                {
                    id++;
                }
                else
                {
                    return id;
                }
            }
        }

        public async Task DeleteCartProduct(int id)
        {
            if (await authenticationStateProvider.IsUserAuthenticated())
            {
                HttpResponseMessage response = await client.DeleteAsync(uri + $"/{id}");
                string content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                }
            }
            else
            {
                List<CartProductUIModel> products = await localStorage.GetItemAsync<List<CartProductUIModel>>(cartProductsKey);
                products.Remove(products.Find(x => x.Id == id));
                await localStorage.SetItemAsync(cartProductsKey, products);
            }
        }

        public async Task ClearCartProducts()
        {
            if (await authenticationStateProvider.IsUserAuthenticated())
            {
                HttpResponseMessage response = await client.DeleteAsync(uri);
                string content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                }
            }
            else
            {
                await localStorage.RemoveItemAsync(cartProductsKey);
            }
        }

        public async Task MarkCartProductCollected(int id)
        {
            if (await authenticationStateProvider.IsUserAuthenticated())
            {
                HttpResponseMessage response = await client.PatchAsync(uri + $"/{id}", null);
                string content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                }
            }
            else
            {
                List<CartProductUIModel> products = await localStorage.GetItemAsync<List<CartProductUIModel>>(cartProductsKey);
                CartProductUIModel product = products.Find(x => x.Id == id);
                product.IsCollected = !product.IsCollected;
                await localStorage.SetItemAsync(cartProductsKey, products);
            }
        }

        internal async Task UpdateCartProduct(CartProductUIModel cartProduct)
        {
            if (await authenticationStateProvider.IsUserAuthenticated())
            {
                HttpResponseMessage response = await client.PutAsJsonAsync(uri + $"/{cartProduct.Id}", cartProduct);
                string content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                }
            }
            else
            {
                List<CartProductUIModel> products = await localStorage.GetItemAsync<List<CartProductUIModel>>(cartProductsKey);
                CartProductUIModel product = products.Find(x => x.Id == cartProduct.Id);
                product.Name = cartProduct.Name;
                product.Amount = cartProduct.Amount;
                product.UnitPrice = cartProduct.UnitPrice;
                await localStorage.SetItemAsync(cartProductsKey, products);
            }
        }
    }
}
