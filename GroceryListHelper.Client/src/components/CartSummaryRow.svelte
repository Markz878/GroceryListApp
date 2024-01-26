<script lang="ts">
  import { derived } from "svelte/store";
  import { cartProducts, checkError, showOnlyUncollected, storeProducts } from "../helpers/store";
  import Confirm from "./Confirm.svelte";
  import { onMount } from "svelte";
  import type { ICartProductsService } from "../types/ICartProductsService";
  import type { IStoreProductsService } from "../types/IStoreProductsService";
  import { getCartProductsService } from "../services/CartProductsServiceProvider";
  import { getStoreProductsService } from "../services/StoreProductsServiceProvider";

  let confirmMessage: string;
  let confirmCallback: () => void;
  let confirmDialog: Confirm;
  let cartProductsService: ICartProductsService;
  let storeProductsService: IStoreProductsService;

  onMount(async () => {
    cartProductsService = await getCartProductsService();
    storeProductsService = await getStoreProductsService();
  });

  let total = derived(cartProducts, ($cartProducts) => {
    return $cartProducts.reduce((x, c) => x + c.unitPrice * c.amount, 0);
  });

  function showDeleteConfirm(message: string, func: () => void) {
    confirmMessage = message;
    confirmCallback = func;
    confirmDialog.showModal();
  }

  function setFilterCollected(e: Event) {
    if (e.target instanceof HTMLInputElement) {
      showOnlyUncollected.set(e.target.checked);
      $showOnlyUncollected = $showOnlyUncollected;
    }
  }

  async function clearCartProducts() {
    cartProducts.set([]);
    checkError(await cartProductsService?.deleteAllCartProducts());
  }

  async function clearStoreProducts() {
    storeProducts.set([]);
    checkError(await storeProductsService?.deleteStoreProducts());
  }
</script>

<div class="grid grid-cols-2 grid-rows-2 md:grid-cols-4 md:grid-rows-1 place-items-center gap-2 pb-4">
  <div class="flex mt-1 ml-4">
    <input id="filter-collected-checkbox" type="checkbox" class="scale-150" on:change={(e) => setFilterCollected(e)} />
    <label id="filter-collected-label" class="ml-3 mb-[2.75px] font-bold whitespace-nowrap" for="filter-collected-checkbox">Filter collected</label>
  </div>
  <b id="cart-total">Total: {$total} €</b>
  <button class="btn btn-primary w-24" on:click={() => showDeleteConfirm("Delete all current cart items?", clearCartProducts)}>Clear cart</button>
  <button class="btn btn-primary w-24" on:click={() => showDeleteConfirm("Delete all stored shop items?", clearStoreProducts)}>Clear shop</button>
</div>
<Confirm header="Confirm delete" message={confirmMessage} on:ok={confirmCallback} bind:this={confirmDialog} />
