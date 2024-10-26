<script lang="ts">
  import { onMount } from "svelte";
  import { getNewOrder } from "../helpers/sortOrderMethods";
  import { cartProducts, showOnlyUncollected, sortState, storeProducts, showError, sortCartProducts, checkError } from "../helpers/store";
  import { CartProduct } from "../types/CartProduct";
  import type { ICartProductsService } from "../types/ICartProductsService";
  import { getCartProductsService } from "../services/CartProductsServiceProvider";
  import { getStoreProductsService } from "../services/StoreProductsServiceProvider";
  import type { IStoreProductsService } from "../types/IStoreProductsService";
  import type { StoreProduct } from "../types/StoreProducts";
  import { derived } from "svelte/store";

  let newProduct = $state(new CartProduct());
  let editingItem = $state<CartProduct | null>();
  let movingItem = $state<CartProduct | null>();
  let newProductNameBox = $state<HTMLInputElement>();

  let cartProductService: ICartProductsService | null;
  let storeProductService: IStoreProductsService | null;

  onMount(async () => {
    cartProductService = await getCartProductsService();
    storeProductService = await getStoreProductsService();
    const cartProductsResponse = await cartProductService.getCartProducts();
    if (cartProductsResponse instanceof Error) {
      showError(cartProductsResponse.message);
    } else {
      cartProducts.set(cartProductsResponse);
    }
    const storeProductsResponse = await storeProductService.getStoreProducts();
    if (storeProductsResponse instanceof Error) {
      showError(storeProductsResponse.message);
    } else {
      storeProducts.set(storeProductsResponse);
    }
  });

  const cartProductsFilteredList = derived([cartProducts, showOnlyUncollected], ([$cartProducts, $showOnlyUncollected]) => {
    const result: { product: CartProduct; top: number }[] = [];
    const filtered = $cartProducts.filter((x) => !$showOnlyUncollected || !x.isCollected);
    const sorted = filtered.toSorted((a, b) => a.order - b.order);
    const topMap = new Map<string, number>();
    for (let i = 0; i < sorted.length; i++) {
      topMap.set(sorted[i]!.name, 0.5 + 3 * i);
    }
    for (const p of filtered) {
      result.push({ product: p, top: topMap.get(p.name) ?? 0 });
    }
    return result;
  });

  function getItemPrice() {
    const product = $storeProducts.find((x) => x.name == newProduct.name);
    if (product && product.unitPrice > 0) {
      newProduct.unitPrice = product.unitPrice;
    }
  }

  async function changeSortDirectionAndSortItems() {
    sortState.update((x) => {
      if (x === "Ascending") {
        return "Descending";
      }
      return "Ascending";
    });
    const sortDirection = $sortState === "Ascending" ? "Ascending" : "Descending";
    sortCartProducts(sortDirection);
    checkError(await cartProductService?.sortCartProducts(sortDirection));
  }

  async function addNewProduct() {
    if (newProduct.name.length === 0) {
      showError("Product name not given");
      return;
    } else if ($cartProducts.some((x) => x.name === newProduct.name)) {
      showError(`Product ${newProduct.name} is already in cart`);
      return;
    } else if (newProduct.amount < 0 || newProduct.amount > 1000) {
      showError("Amount must be between 0 and 10 000");
      return;
    } else if (newProduct.unitPrice < 0 || newProduct.unitPrice > 1000) {
      showError("Price must be between 0 and 10 000");
      return;
    } else if ($cartProducts.length > 150) {
      showError("Cart can have a maximum of 150 items");
      return;
    }
    const cartProduct = { ...newProduct };
    newProduct = new CartProduct();
    await saveCartProduct(cartProduct);
    await saveStoreProduct(cartProduct);
    newProductNameBox?.focus();
  }

  async function saveCartProduct(product: CartProduct) {
    product.order = getNewCartProductOrder();
    cartProducts.update((x) => [...x, product]);
    checkError(await cartProductService?.createCartProduct(product));
  }

  async function saveStoreProduct(product: StoreProduct) {
    const existingProduct = $storeProducts.find((x) => x.name == product.name);
    if (existingProduct) {
      if (existingProduct.unitPrice !== product.unitPrice) {
        existingProduct.unitPrice = product.unitPrice;
        checkError(await storeProductService?.updateStoreProduct(existingProduct));
      }
    } else {
      storeProducts.update((x) => [...x, { name: product.name, unitPrice: product.unitPrice }]);
      checkError(await storeProductService?.createStoreProduct(product));
    }
  }

  function getNewCartProductOrder() {
    if ($cartProducts.length === 0) {
      return 1000;
    }
    const order = Math.round(Math.max(...$cartProducts.map((x) => x.order)) + 1000);
    return order;
  }

  function getRowClass(product: CartProduct) {
    return product.isCollected ? "bg-gray-400 dark:bg-gray-600" : "";
  }

  async function markItemCollected(product: CartProduct, e: Event) {
    if (e.target instanceof HTMLInputElement) {
      product.isCollected = e.target.checked;
      cartProducts.update((x) => x);
      checkError(await cartProductService?.updateCartProduct(product));
    }
  }

  function startEditItem(product: CartProduct) {
    editingItem = product;
  }

  async function removeProduct(product: CartProduct) {
    cartProducts.update((p) => p.filter((f) => f.name !== product.name));
    checkError(await cartProductService?.deleteCartProduct(product.name));
  }

  async function updateCartProduct(product: CartProduct) {
    editingItem = null;
    $cartProducts = $cartProducts;
    await cartProductService?.updateCartProduct(product);
    const storeProduct = $storeProducts.find((x) => x.name == product.name);
    if (storeProduct && storeProduct.unitPrice !== product.unitPrice) {
      storeProduct.unitPrice = product.unitPrice;
      checkError(await storeProductService?.updateStoreProduct(storeProduct));
    }
  }

  async function move(product: CartProduct) {
    if (!movingItem) {
      movingItem = product;
    } else if (product === movingItem) {
      movingItem = null;
    } else {
      sortState.set("None");
      cartProducts.update((cartProducts) => {
        for (const p of cartProducts) {
          if (p.name === movingItem?.name) {
            p.order = getNewOrder(
              cartProducts.map((x) => x.order),
              movingItem.order,
              product.order
            );
          }
        }
        return [...cartProducts];
      });
      checkError(await cartProductService?.updateCartProduct(movingItem));
      movingItem = null;
    }
  }

  async function setProductAmount(e: Event, product: CartProduct) {
    if (e.target instanceof HTMLInputElement) {
      product.amount = parseFloat(e.target.value);
    }
  }

  async function setProductPrice(e: Event, product: CartProduct) {
    if (e.target instanceof HTMLInputElement) {
      product.unitPrice = parseFloat(e.target.value);
    }
  }
</script>

<div role="rowheader" class="grid grid-cols-base sm:grid-cols-sm md:grid-cols-md lg:grid-cols-lg gap-2 justify-center align-middle w-full pt-2 font-semibold">
  <span class="text-center">Reorder</span>
  <span class="text-center">Collected</span>
  <div class="flex justify-center">
    <button onclick={changeSortDirectionAndSortItems} aria-label="Sort items" class="cursor-pointer"> Product </button>
    {#if $sortState !== "None"}
      <img class="scale-50 h-6 dark:invert" alt={$sortState == "Ascending" ? "sort down" : "sort up"} src={$sortState == "Ascending" ? "icons/arrow-down.svg" : "icons/arrow-up.svg"} />
    {/if}
  </div>
  <span class="text-center hidden md:block">Amount</span>
  <span class="text-center hidden sm:block">Price</span>
  <span class="text-center hidden lg:block">Total</span>
  <span></span>

  <span></span>
  <div class="flex justify-center"><button id="add-cartproduct-button" type="submit" class="btn btn-success" onclick={addNewProduct} aria-label="Add product">Add</button></div>
  <span>
    <input id="newproduct-name-input" type="text" list="products" class="form-control text-center" aria-label="Product name input" autocomplete="off" bind:value={newProduct.name} onfocusout={getItemPrice} bind:this={newProductNameBox} />
    <datalist id="products">
      {#each $storeProducts.filter((s) => $cartProducts.every((c) => c.name !== s.name)) as storeProduct}
        <option value={storeProduct.name}></option>
      {/each}
    </datalist>
  </span>
  <input id="newproduct-amount-input" type="number" step="1" min="1" class="form-control text-center hidden md:block" aria-label="Product amount input" bind:value={newProduct.amount} />
  <input id="newproduct-price-input" type="number" step="0.01" min="0" class="form-control text-center hidden sm:block" aria-label="Product unit price input" bind:value={newProduct.unitPrice} />
  <span></span>
</div>

<div role="rowgroup" class="relative transition-[height]" style="height: {$cartProducts.filter((x) => !$showOnlyUncollected || !x.isCollected).length * 3 + 1}rem;">
  {#each $cartProductsFilteredList as cartProduct (cartProduct.product.name)}
    <div role="row" class="absolute h-12 w-full grid grid-cols-base sm:grid-cols-sm md:grid-cols-md lg:grid-cols-lg transition-[top] motion-reduce:transition-none border-t-2 {getRowClass(cartProduct.product)}" style="top: {cartProduct.top}rem;">
      {#if cartProduct.product !== editingItem}
        <button class="btn btn-primary w-9 h-9 p-0 m-auto {cartProduct.product == movingItem ? 'bg-blue-800' : ''}" aria-label="Reorder" onclick={() => move(cartProduct.product)}>
          <img class="m-auto invert w-7 h-7" src="icons/swap.svg" alt="Swap" aria-hidden="true" />
        </button>
        <input type="checkbox" class="scale-150 m-auto" checked={cartProduct.product.isCollected} aria-label="Mark collected" onchange={(e) => markItemCollected(cartProduct.product, e)} />
        <span class="m-auto {cartProduct.product.isCollected ? 'line-through' : ''}" aria-label="Product name">{cartProduct.product.name}</span>
        <span class="m-auto hidden md:block" aria-label="Amount">{cartProduct.product.amount}</span>
        <span class="m-auto hidden sm:block" aria-label="Unit price">{cartProduct.product.unitPrice}</span>
        <span class="m-auto hidden lg:block" aria-label="Total price">{cartProduct.product.unitPrice * cartProduct.product.amount}</span>
        <div class="flex">
          <button class="btn btn-success w-9 h-9 p-0 m-auto hidden sm:block" onclick={() => startEditItem(cartProduct.product)} aria-label="Edit product" disabled={cartProduct.product.isCollected}>
            <img class="m-auto invert w-6 h-6" src="icons/edit.svg" alt="Edit" aria-hidden="true" />
          </button>
          <button class="btn btn-danger w-9 h-9 p-0 m-auto" onclick={() => removeProduct(cartProduct.product)} aria-label="Delete product" disabled={cartProduct.product.isCollected}>
            <img class="m-auto invert w-6 h-6" src="icons/delete.svg" alt="Delete" aria-hidden="true" />
          </button>
        </div>
      {:else}
        <span></span>
        <span></span>
        <span class="m-auto {cartProduct.product.isCollected ? 'line-through' : ''}" aria-label="Product name">{cartProduct.product.name}</span>
        <input type="number" step="0.01" min="0" class="form-control text-center m-auto hidden md:block" aria-label="Edit amount" value={cartProduct.product.amount} onchange={(e) => setProductAmount(e, cartProduct.product)} />
        <input type="number" step="0.01" min="0" class="form-control text-center my-auto mx-2 hidden sm:block" aria-label="Edit unit price" value={cartProduct.product.unitPrice} onchange={(e) => setProductPrice(e, cartProduct.product)} />
        <span class="m-auto hidden lg:block" aria-label="Total price">{cartProduct.product.unitPrice * cartProduct.product.amount}</span>
        <button class="btn btn-success m-auto h-9 w-9 p-1" onclick={() => updateCartProduct(cartProduct.product)} aria-label="Submit edit">
          <img class="m-auto invert" src="icons/check.svg" alt="Accept" aria-hidden="true" />
        </button>
      {/if}
    </div>
  {/each}
</div>
