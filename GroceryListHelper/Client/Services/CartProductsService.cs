using GroceryListHelper.Client.Models;
using GroceryListHelper.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static GroceryListHelper.Client.HelperMethods.HelperMethods;

namespace GroceryListHelper.Client.Services
{
    public class CartProductsService
    {
        private readonly HttpClient client;
        private const string uri = "api/cartproducts";

        public CartProductsService(IHttpClientFactory clientFactory)
        {
            client = clientFactory.CreateClient("ProtectedClient");
        }

        public async Task<List<CartProductUIModel>> GetCartProducts()
        {
            try
            {
                return await client.GetFromJsonAsync<List<CartProductUIModel>>(uri);
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
                return new List<CartProductUIModel>();
            }
        }

        public async Task SaveCartProduct(CartProductUIModel product)
        {
            try
            {
                var response = await client.PostAsJsonAsync(uri, product as CartProduct);
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    product.Id = int.Parse(content);
                }
                else
                {
                    throw new InvalidOperationException(content);
                }
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }

        public Task DeleteCartProduct(int id)
        {
            return CheckAccessToken(client.DeleteAsync(uri + $"/{id}"));
        }

        public Task ClearCartProducts()
        {
            return CheckAccessToken(client.DeleteAsync(uri));
        }

        public Task MarkCartProductCollected(int id)
        {
            return CheckAccessToken(client.PatchAsync(uri + $"/{id}", null));
        }

        internal Task UpdateCartProduct(CartProductUIModel cartProduct)
        {
            return CheckAccessToken(client.PutAsJsonAsync(uri + $"/{cartProduct.Id}", cartProduct));
        }
    }
}
