using System.ComponentModel;
using System.Diagnostics;

namespace GroceryListHelper.Client.Components;

public abstract class CartComponentBase : BasePage<MainViewModel>
{
    [Inject] public required ModalViewModel ModalViewModel { get; set; }
    [Inject] public required ICartProductsService CartProductsService { get; set; }
    [Inject] public required IStoreProductsService StoreProductsService { get; set; }
    [Inject] public required PersistentComponentState ApplicationState { get; set; }

    private PersistingComponentStateSubscription stateSubscription;

    protected CartProduct newProduct = new();
    protected CartProductUIModel? editingItem;
    protected CartProductUIModel? movingItem;
    protected ElementReference NewProductNameBox;
    protected SortState sortState;
    protected bool isAuthenticated;

    protected override async Task OnInitializedAsync()
    {
        if (ApplicationState.TryTakeFromJson(nameof(ViewModel.CartProducts), out IList<CartProductUIModel>? cartProducts) && cartProducts is not null && cartProducts.Count > 0 && ApplicationState.TryTakeFromJson(nameof(ViewModel.StoreProducts), out IList<StoreProduct>? storeProducts) && storeProducts is not null)
        {
            ViewModel.CartProducts.Clear();
            foreach (CartProductUIModel item in cartProducts)
            {
                ViewModel.CartProducts.Add(item);
            }
            ViewModel.StoreProducts.Clear();
            foreach (StoreProduct item in storeProducts)
            {
                ViewModel.StoreProducts.Add(item);
            }
        }
        else
        {
            ViewModel.CartProducts.Clear();
            foreach (CartProductUIModel item in await CartProductsService.GetCartProducts())
            {
                ViewModel.CartProducts.Add(item);
            }
            ViewModel.StoreProducts.Clear();
            foreach (StoreProduct item in await StoreProductsService.GetStoreProducts())
            {
                ViewModel.StoreProducts.Add(item);
            }
        }
        stateSubscription = ApplicationState.RegisterOnPersisting(PersistData);
    }

    private Task PersistData()
    {
        ApplicationState?.PersistAsJson(nameof(ViewModel.CartProducts), ViewModel.CartProducts);
        ApplicationState?.PersistAsJson(nameof(ViewModel.StoreProducts), ViewModel.StoreProducts);
        return Task.CompletedTask;
    }

    protected async Task SortItems()
    {
        if (ViewModel.IsPolling)
        {
            return;
        }
        sortState = sortState switch
        {
            SortState.None => SortState.Ascending,
            SortState.Ascending => SortState.Descending,
            SortState.Descending => SortState.Ascending,
            _ => throw new UnreachableException()
        };
        int order = 1000;
        if (sortState == SortState.Ascending)
        {
            foreach (CartProductUIModel? item in ViewModel.CartProducts.OrderBy(x => x.Name))
            {
                item.Order = order;
                order += 1000;
            }
        }
        else if (sortState == SortState.Descending)
        {
            foreach (CartProductUIModel? item in ViewModel.CartProducts.OrderByDescending(x => x.Name))
            {
                item.Order = order;
                order += 1000;
            }
        }
        await CartProductsService.SortCartProducts(sortState == SortState.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending);
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
            ModalViewModel.Header = "Error";
            ModalViewModel.Message = ex.Message;
        }
    }

    public async Task SaveCartProduct(CartProduct product)
    {
        CartProductClientValidator cartProductValidator = new(ViewModel.CartProducts);
        ValidationResult validationResult = cartProductValidator.Validate(product);
        if (!validationResult.IsValid)
        {
            throw new Exception(string.Join(" ", validationResult.Errors.Select(x => x.ErrorMessage)));
        }
        product.Order = GetNewCartProductOrder(ViewModel.CartProducts);
        CartProductUIModel newCartProductUIModel = new()
        {
            Amount = product.Amount,
            Name = product.Name,
            Order = product.Order,
            UnitPrice = product.UnitPrice
        };
        ViewModel.CartProducts.Add(newCartProductUIModel);
        try
        {
            await CartProductsService.SaveCartProduct(product);
        }
        catch
        {
            ViewModel.CartProducts.Remove(newCartProductUIModel);
            throw;
        }
    }

    public static string GetRowClass(CartProductUIModel cartProduct)
    {
        return cartProduct.IsCollected ? "checked-item" : "";
    }

    public async Task MarkItemCollected(ChangeEventArgs e, CartProductUIModel product)
    {
        product.IsCollected = (bool)e.Value!;
        ViewModel.OnPropertyChanged();
        await CartProductsService.UpdateCartProduct(product);
    }

    public async Task SaveStoreProduct(StoreProduct storeProduct)
    {
        StoreProduct? product = ViewModel.StoreProducts.FirstOrDefault(x => x.Name == storeProduct.Name);
        if (product == null)
        {
            StoreProductClientValidator storeProductValidator = new(ViewModel.StoreProducts);
            ValidationResult validationResult = storeProductValidator.Validate(storeProduct);
            if (!validationResult.IsValid)
            {
                throw new Exception(string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage)));
            }
            try
            {
                await StoreProductsService.SaveStoreProduct(storeProduct);
                product = new()
                {
                    Name = storeProduct.Name,
                    UnitPrice = storeProduct.UnitPrice
                };
                ViewModel.StoreProducts.Add(product);
            }
            catch
            {
                throw;
            }
        }
        else
        {
            if (product.UnitPrice != storeProduct.UnitPrice)
            {
                product.UnitPrice = storeProduct.UnitPrice;
                await StoreProductsService.UpdateStoreProduct(product);
            }
        }
    }

    public void StartEditItem(CartProductUIModel product)
    {
        editingItem = product;
    }

    public async Task UpdateCartProduct(CartProductUIModel product)
    {
        CartProductClientValidator cartProductValidator = new(ViewModel.CartProducts);
        ModalViewModel.Message = string.Join(" ", cartProductValidator.Validate(product).Errors.Select(x => x.ErrorMessage));
        if (string.IsNullOrEmpty(ModalViewModel.Message))
        {
            editingItem = null;
            ViewModel.OnPropertyChanged();
            try
            {
                await CartProductsService.UpdateCartProduct(product);
                List<StoreProduct> storeProducts = await StoreProductsService.GetStoreProducts();
                StoreProduct? storeProduct = storeProducts.FirstOrDefault(x => x.Name == product.Name);
                if (storeProduct is not null && storeProduct.UnitPrice != product.UnitPrice)
                {
                    storeProduct.UnitPrice = product.UnitPrice;
                    await StoreProductsService.UpdateStoreProduct(storeProduct);
                }
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
        StoreProduct? product = ViewModel.StoreProducts.FirstOrDefault(x => x.Name == newProduct.Name);
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
        else if (cartProduct == movingItem)
        {
            movingItem = null;
        }
        else
        {
            sortState = SortState.None;
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

    public async Task RemoveProduct(CartProductUIModel product)
    {
        ViewModel.CartProducts.Remove(product);
        try
        {
            await CartProductsService.DeleteCartProduct(product.Name);
        }
        catch (Exception ex)
        {
            ModalViewModel.Message = ex.Message;
        }
    }

    public override void Dispose()
    {
        stateSubscription.Dispose();
        base.Dispose();
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
