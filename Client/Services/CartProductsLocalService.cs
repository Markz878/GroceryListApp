﻿using Blazored.LocalStorage;
using GroceryListHelper.Client.Models;

namespace GroceryListHelper.Client.Services;

public class CartProductsLocalService : ICartProductsService
{
    private readonly ILocalStorageService localStorage;
    private const string cartProductsKey = "cartProducts";

    public CartProductsLocalService(ILocalStorageService localStorage)
    {
        this.localStorage = localStorage;
    }

    public async Task<List<CartProductUIModel>> GetCartProducts()
    {
        return await localStorage.GetItemAsync<List<CartProductUIModel>>(cartProductsKey) ?? new List<CartProductUIModel>();
    }

    public async Task SaveCartProduct(CartProductUIModel product)
    {
        List<CartProductUIModel> products = await localStorage.GetItemAsync<List<CartProductUIModel>>(cartProductsKey) ?? new List<CartProductUIModel>();
        products.Add(product);
        await localStorage.SetItemAsync(cartProductsKey, products);
    }

    public async Task DeleteCartProduct(string id)
    {
        List<CartProductUIModel> products = await localStorage.GetItemAsync<List<CartProductUIModel>>(cartProductsKey);
        products.Remove(products.Find(x => x.Id == id));
        await localStorage.SetItemAsync(cartProductsKey, products);
    }

    public async Task DeleteAllCartProducts()
    {
        await localStorage.RemoveItemAsync(cartProductsKey);
    }

    //public async Task MarkCartProductCollected(string id)
    //{
    //    List<CartProductUIModel> products = await localStorage.GetItemAsync<List<CartProductUIModel>>(cartProductsKey);
    //    CartProductUIModel product = products.Find(x => x.Id == id);
    //    product.IsCollected = !product.IsCollected;
    //    await localStorage.SetItemAsync(cartProductsKey, products);
    //}

    public async Task UpdateCartProduct(CartProductUIModel cartProduct)
    {
        List<CartProductUIModel> products = await localStorage.GetItemAsync<List<CartProductUIModel>>(cartProductsKey);
        CartProductUIModel product = products.Find(x => x.Id == cartProduct.Id);
        product.Name = cartProduct.Name;
        product.Amount = cartProduct.Amount;
        product.UnitPrice = cartProduct.UnitPrice;
        product.IsCollected = cartProduct.IsCollected;
        product.Order = cartProduct.Order;
        await localStorage.SetItemAsync(cartProductsKey, products);
    }

    //public async Task CartItemMoved(CartProductUIModel cartProduct, int newIndex)
    //{
    //    List<CartProductUIModel> products = await localStorage.GetItemAsync<List<CartProductUIModel>>(cartProductsKey);
    //    CartProductUIModel product = products.Find(x => x.Id == cartProduct.Id);
    //    products.Remove(product);
    //    products.Insert(newIndex, product);
    //    await localStorage.SetItemAsync(cartProductsKey, products);
    //}
}