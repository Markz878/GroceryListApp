using FluentValidation.Results;
using GroceryListHelper.Client.Features.CartProducts;
using GroceryListHelper.Client.Features.StoreProducts;
using GroceryListHelper.Client.Validators;
using System.Diagnostics;

namespace GroceryListHelper.Client.Components;

public partial class CartComponent
{
    [CascadingParameter] public required AppState AppState { get; init; }
    [Inject] public required IMediator Mediator { get; init; }
    [Parameter] public bool ShowLoading { get; init; }

    private CartProduct newProduct = new();
    private CartProductCollectable? editingItem;
    private CartProductCollectable? movingItem;
    private ElementReference NewProductNameBox;

    private async Task ChangeSortDirectionAndSortItems()
    {
        ChangeSortDirection();
        await SortItems();
    }

    private void ChangeSortDirection()
    {
        AppState.SortDirection = AppState.SortDirection switch
        {
            SortState.None => SortState.Ascending,
            SortState.Ascending => SortState.Descending,
            SortState.Descending => SortState.Ascending,
            _ => throw new UnreachableException()
        };
    }

    private async Task SortItems()
    {
        if (AppState.SortDirection == SortState.None)
        {
            return;
        }
        ListSortDirection sortDirection = AppState.SortDirection == SortState.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending;
        ProductSortMethods.SortProducts(AppState.CartProducts, sortDirection);
        await Mediator.Send(new SortCartProductsCommand() { SortDirection = sortDirection });
    }

    private static double GetNewCartProductOrder(IEnumerable<CartProductCollectable> cartProducts)
    {
        if (!cartProducts.Any())
        {
            return 1000;
        }
        double order = Math.Round(cartProducts.Max(x => x.Order) + 1000, 0, MidpointRounding.AwayFromZero);
        return order;
    }

    private async Task AddNewProduct()
    {
        try
        {
            CartProduct product = newProduct with { };
            newProduct = new();
            await SaveCartProduct(product);
            await SaveStoreProduct(product);
            await NewProductNameBox.FocusAsync();
            await SortItems();
        }
        catch (Exception ex)
        {
            AppState.ShowError(ex.Message);
        }
    }

    private async Task SaveCartProduct(CartProduct product)
    {
        CartProductClientValidator cartProductValidator = new(AppState.CartProducts);
        ValidationResult validationResult = cartProductValidator.Validate(product);
        if (!validationResult.IsValid)
        {
            throw new Exception(string.Join(" ", validationResult.Errors.Select(x => x.ErrorMessage)));
        }
        product.Order = GetNewCartProductOrder(AppState.CartProducts);
        CartProductCollectable newCartProductCollectable = new()
        {
            Amount = product.Amount,
            Name = product.Name,
            Order = product.Order,
            UnitPrice = product.UnitPrice
        };
        AppState.CartProducts.Add(newCartProductCollectable);
        try
        {
            await Mediator.Send(new CreateCartProductCommand() { Product = product });
        }
        catch
        {
            AppState.CartProducts.Remove(newCartProductCollectable);
            throw;
        }
    }

    private static string GetRowClass(CartProductCollectable cartProduct)
    {
        return cartProduct.IsCollected ? "bg-gray-400 dark:bg-gray-600" : "";
    }

    private async Task MarkItemCollected(ChangeEventArgs e, CartProductCollectable product)
    {
        product.IsCollected = (bool)e.Value!;
        await InvokeAsync(StateHasChanged);
        await Mediator.Send(new UpdateCartProductCommand() { Product = product });
    }

    private async Task SaveStoreProduct(StoreProduct storeProduct)
    {
        StoreProduct? product = AppState.StoreProducts.FirstOrDefault(x => x.Name == storeProduct.Name);
        if (product == null)
        {
            StoreProductClientValidator storeProductValidator = new(AppState.StoreProducts);
            ValidationResult validationResult = storeProductValidator.Validate(storeProduct);
            if (!validationResult.IsValid)
            {
                throw new Exception(string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage)));
            }
            product = storeProduct with { };
            AppState.StoreProducts.Add(product);
            await Mediator.Send(new CreateStoreProductCommand() { Product = product });
        }
        else
        {
            if (product.UnitPrice != storeProduct.UnitPrice)
            {
                product.UnitPrice = storeProduct.UnitPrice;
                await Mediator.Send(new UpdateStoreProductCommand() { Product = product });
            }
        }
    }

    private void StartEditItem(CartProductCollectable product)
    {
        editingItem = product;
    }

    private async Task UpdateCartProduct(CartProductCollectable product)
    {
        CartProductClientValidator cartProductValidator = new(AppState.CartProducts);
        ValidationResult validationResult = cartProductValidator.Validate(product);
        if (validationResult.IsValid)
        {
            editingItem = null;
            AppState.OnPropertyChanged();
            try
            {
                await Mediator.Send(new UpdateCartProductCommand() { Product = product });
                StoreProduct? storeProduct = AppState.StoreProducts.FirstOrDefault(x => x.Name == product.Name);
                if (storeProduct is not null && storeProduct.UnitPrice != product.UnitPrice)
                {
                    storeProduct.UnitPrice = product.UnitPrice;
                    await Mediator.Send(new UpdateStoreProductCommand() { Product = product });
                }
            }
            catch (Exception ex)
            {
                AppState.ShowError(ex.Message);
            }
        }
        else
        {
            AppState.ShowError(string.Join(" ", validationResult.Errors.Select(x => x.ErrorMessage)));
        }
    }

    private void GetItemPrice()
    {
        StoreProduct? product = AppState.StoreProducts.FirstOrDefault(x => x.Name == newProduct.Name);
        if (product?.UnitPrice > 0)
        {
            newProduct.UnitPrice = product.UnitPrice;
        }
    }

    private async Task Move(CartProductCollectable cartProduct)
    {
        if (movingItem == null)
        {
            movingItem = cartProduct;
        }
        else if (cartProduct == movingItem)
        {
            movingItem = null;
        }
        else
        {
            AppState.SortDirection = SortState.None;
            movingItem.Order = SortOrderMethods.GetNewOrder(AppState.CartProducts.Select(x => x.Order), movingItem.Order, cartProduct.Order);
            try
            {
                await Mediator.Send(new UpdateCartProductCommand() { Product = movingItem });
            }
            catch (Exception ex)
            {
                AppState.ShowError(ex.Message);
            }
            finally
            {
                movingItem = null;
            }
        }
    }

    private double GetRowTop(CartProductCollectable cartProduct)
    {
        int productIndex = -1;
        foreach ((CartProductCollectable product, int index) in AppState.CartProducts.Where(x => !AppState.ShowOnlyUncollected || !x.IsCollected).OrderBy(x => x.Order).Select((product, index) => (product, index)))
        {
            if (cartProduct == product)
            {
                productIndex = index;
                break;
            }
        }
        return 0.5 + productIndex * 3;
    }

    private async Task RemoveProduct(CartProductCollectable product)
    {
        AppState.CartProducts.Remove(product);
        try
        {
            await Mediator.Send(new DeleteCartProductCommand() { ProductName = product.Name });
        }
        catch (Exception ex)
        {
            AppState.ShowError(ex.Message);
        }
    }

    //private CartProductCollectable dragTarget;
    //public void DragStarted(CartProductCollectable product)
    //{
    //    dragTarget = product;
    //}

    //public async Task OnDrop(CartProductCollectable product)
    //{
    //    int dragTargetIndex = AppState.CartProducts.IndexOf(dragTarget);
    //    int dropTargetIndex = AppState.CartProducts.IndexOf(product);
    //    if (dragTargetIndex != dropTargetIndex)
    //    {
    //        AppState.CartProducts.Move(dragTargetIndex, dropTargetIndex);
    //        if (AppState.IsPolling)
    //        {
    //            await AppState.CartHub.SendAsync(nameof(ICartHubActions.CartItemMoved), dragTarget.Id, dropTargetIndex);
    //        }
    //    }
    //}
}
