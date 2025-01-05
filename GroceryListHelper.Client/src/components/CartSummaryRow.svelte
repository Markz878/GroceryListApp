<script lang="ts">
  import store from "../helpers/store.svelte";
  import { CartProductsProxyService } from "../services/CartProductsProxyService";
  import { StoreProductsProxyService } from "../services/StoreProductsProxyService";
  import Confirm from "./Confirm.svelte";

  let confirmMessage = $state("");
  let confirmCallback = $state<() => void>();
  let confirmDialog = $state<Confirm>();
  let cartProductsService = new CartProductsProxyService();
  let storeProductsService = new StoreProductsProxyService();

  const total = $derived(store.cartProducts.reduce((x, c) => x + c.unitPrice * c.amount, 0));

  function showDeleteConfirm(message: string, func: () => void) {
    confirmMessage = message;
    confirmCallback = func;
    confirmDialog?.showModal();
  }

  function setFilterCollected(e: Event) {
    if (e.target instanceof HTMLInputElement) {
      store.showOnlyUncollected = e.target.checked;
    }
  }

  async function clearCartProducts() {
    store.cartProducts = [];
    store.checkError(await cartProductsService?.deleteAllCartProducts());
  }

  async function clearStoreProducts() {
    store.storeProducts = [];
    store.checkError(await storeProductsService?.deleteStoreProducts());
  }
</script>

<div class="grid grid-cols-2 grid-rows-2 md:grid-cols-4 md:grid-rows-1 place-items-center gap-2 pb-4">
  <div class="flex mt-1 ml-4">
    <input id="filter-collected-checkbox" type="checkbox" class="scale-150" onchange={(e) => setFilterCollected(e)} />
    <label id="filter-collected-label" class="ml-3 mb-[2.75px] font-bold whitespace-nowrap" for="filter-collected-checkbox">Filter collected</label>
  </div>
  <b id="cart-total">Total: {total} €</b>
  <button class="btn btn-primary w-24" onclick={() => showDeleteConfirm("Delete all current cart items?", clearCartProducts)}>Clear cart</button>
  <button class="btn btn-primary w-24" onclick={() => showDeleteConfirm("Delete all stored shop items?", clearStoreProducts)}>Clear shop</button>
</div>
<Confirm header="Confirm delete" message={confirmMessage} confirm={confirmCallback ?? (() => {})} bind:this={confirmDialog} />
