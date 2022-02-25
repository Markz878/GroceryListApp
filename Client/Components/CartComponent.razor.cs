using GroceryListHelper.Client.HelperMethods;
using GroceryListHelper.Client.Models;
using GroceryListHelper.Client.Services;
using GroceryListHelper.Client.Validators;
using GroceryListHelper.Client.ViewModels;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;

namespace GroceryListHelper.Client.Components;

public class CartComponentBase : BasePage<IndexViewModel>
{
    [Inject] public ModalViewModel ModalViewModel { get; set; }
    [Inject] public ICartProductsService CartProductsService { get; set; }
    [Inject] public IStoreProductsService StoreProductsService { get; set; }

    protected CartProductUIModel newProduct = new();
    protected CartProductUIModel editingItem;
    protected CartProductUIModel movingItem;
    protected ElementReference NewProductNameBox;
    protected ElementReference AddProductButton;

    protected override async Task OnInitializedAsync()
    {
        ViewModel.IsBusy = true;
        ViewModel.CartProducts.Clear();
        foreach (CartProductUIModel item in await CartProductsService.GetCartProducts())
        {
            ViewModel.CartProducts.Add(item);
        }
        ViewModel.StoreProducts.Clear();
        foreach (StoreProductUIModel item in await StoreProductsService.GetStoreProducts())
        {
            ViewModel.StoreProducts.Add(item);
        }
        ViewModel.IsBusy = false;
    }

    private static double GetNewCartProductOrder(ObservableCollection<CartProductUIModel> cartProducts)
    {
        if (cartProducts.Count == 0)
        {
            return 1000;
        }
        double order = Math.Round(cartProducts.Max(x => x.Order) + 1000, 0, MidpointRounding.AwayFromZero);
        return order;
    }

    public async Task AddNewProduct()
    {
        CartProductValidator cartProductValidator = new(ViewModel.CartProducts);
        ModalViewModel.Message = string.Join(" ", cartProductValidator.Validate(newProduct).Errors.Select(x => x.ErrorMessage));
        if (string.IsNullOrEmpty(ModalViewModel.Message))
        {
            try
            {
                newProduct.Order = GetNewCartProductOrder(ViewModel.CartProducts);
                ViewModel.CartProducts.Add(newProduct);
                CartProductUIModel p = newProduct;
                newProduct = new CartProductUIModel();
                await SaveCartProduct(p);
                await SaveStoreProduct(p.Name, p.UnitPrice);
                await NewProductNameBox.FocusAsync();
            }
            catch (Exception ex)
            {
                ModalViewModel.Header = "Error";
                ModalViewModel.Message = ex.Message;
            }
        }
    }

    public async Task SaveCartProduct(CartProductUIModel product)
    {
        try
        {
            await CartProductsService.SaveCartProduct(product);
        }
        catch (Exception ex)
        {
            ModalViewModel.Header = "Error";
            ModalViewModel.Message = ex.Message;
        }
    }

    public Task MarkItemCollected(ChangeEventArgs e, CartProductUIModel product)
    {
        product.IsCollected = (bool)e.Value;
        ViewModel.OnPropertyChanged();
        return CartProductsService.UpdateCartProduct(product);
    }

    public async Task SaveStoreProduct(string productName, double unitPrice)
    {
        StoreProductUIModel product = ViewModel.StoreProducts.FirstOrDefault(x => x.Name == productName);
        if (product != null)
        {
            if (product.UnitPrice != unitPrice)
            {
                product.UnitPrice = unitPrice;
                bool success = await StoreProductsService.UpdateStoreProductPrice(product);
                if (!success)
                {
                    ModalViewModel.Message = "Could not update store product.";
                }
            }
        }
        else
        {
            StoreProductValidator storeProductValidator = new(ViewModel.StoreProducts);
            product = new StoreProductUIModel() { Name = productName, UnitPrice = unitPrice };
            if (storeProductValidator.Validate(product).IsValid)
            {
                ViewModel.StoreProducts.Add(product);
                bool success = await StoreProductsService.SaveStoreProduct(product);
                if (!success)
                {
                    ModalViewModel.Message = "Could not create store product.";
                }
            }
        }
    }

    public void StartEditItem(CartProductUIModel product)
    {
        editingItem = product;
    }

    public async Task UpdateCartProduct(CartProductUIModel product)
    {
        CartProductValidator cartProductValidator = new(ViewModel.CartProducts);
        ModalViewModel.Message = string.Join(" ", cartProductValidator.Validate(product).Errors.Select(x => x.ErrorMessage));
        if (string.IsNullOrEmpty(ModalViewModel.Message))
        {
            editingItem = null;
            ViewModel.OnPropertyChanged();
            try
            {
                await CartProductsService.UpdateCartProduct(product);
            }
            catch (Exception ex)
            {
                ModalViewModel.Message = ex.Message;
            }
        }
    }

    public void CancelProductUpdate()
    {
        editingItem = null;
    }

    public void GetItemPrice()
    {
        StoreProductUIModel product = ViewModel.StoreProducts.FirstOrDefault(x => x.Name == newProduct.Name);
        if (product?.UnitPrice > 0)
        {
            newProduct.UnitPrice = product.UnitPrice;
        }
    }

    public async Task Move(CartProductUIModel cartProduct)
    {
        if (movingItem == null)
        {
            movingItem = cartProduct;
        }
        else
        {
            if (cartProduct != movingItem)
            {
                movingItem.Order = SortOrderMethods.GetNewOrder(ViewModel.CartProducts.Select(x => x.Order), movingItem.Order, cartProduct.Order);
                try
                {
                    await CartProductsService.UpdateCartProduct(movingItem);
                }
                catch (Exception ex)
                {
                    ModalViewModel.Message = ex.Message;
                }
                finally
                {
                    movingItem = null;
                }
            }
        }
    }

    public async Task RemoveProduct(CartProductUIModel product)
    {
        ViewModel.CartProducts.Remove(product);
        try
        {
            await CartProductsService.DeleteCartProduct(product.Id);
        }
        catch (Exception ex)
        {
            ModalViewModel.Message = ex.Message;
        }
    }

    //private CartProductUIModel dragTarget;
    //public void DragStarted(CartProductUIModel product)
    //{
    //    dragTarget = product;
    //}

    //public async Task OnDrop(CartProductUIModel product)
    //{
    //    int dragTargetIndex = ViewModel.CartProducts.IndexOf(dragTarget);
    //    int dropTargetIndex = ViewModel.CartProducts.IndexOf(product);
    //    if (dragTargetIndex != dropTargetIndex)
    //    {
    //        ViewModel.CartProducts.Move(dragTargetIndex, dropTargetIndex);
    //        if (ViewModel.IsPolling)
    //        {
    //            await ViewModel.CartHub.SendAsync(nameof(ICartHubActions.CartItemMoved), dragTarget.Id, dropTargetIndex);
    //        }
    //    }
    //}
}
