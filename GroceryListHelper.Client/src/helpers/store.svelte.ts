import type { StoreProduct } from '../types/StoreProducts';
import type { CartProduct } from '../types/CartProduct';
import type { CartSortState, SortDirection } from '../types/SortState';
import type { ModalInfo } from '../types/ModalInfo';
import { UserInfo } from '../types/UserInfo';

class Store {
  authInfo = $state<UserInfo>();
  cartProducts = $state<CartProduct[]>([]);
  storeProducts = $state<StoreProduct[]>([]);
  sortState = $state<CartSortState>("None");
  showOnlyUncollected = $state<boolean>(false);
  modalState = $state<ModalInfo>({ header: null, message: null })
  isSharing = $state<boolean>(false);

  updateCartProduct = (product: CartProduct) => {
    const p = this.cartProducts.find(x => x.name === product.name);
    if (p) {
      p.amount = product.amount;
      p.isCollected = product.isCollected;
      p.unitPrice = product.unitPrice;
      if (p.order !== product.order) {
        p.order = product.order;
        this.sortState = "None";
      }
    }
  }

  sortCartProducts = (sortDirection: SortDirection) => {
    let order = 1000;
    if (sortDirection === "Ascending") {
      for (const product of this.cartProducts.toSorted((a, b) => a.name.localeCompare(b.name))) {
        product.order = order;
        order += 1000;
      }
    }
    else {
      for (const product of this.cartProducts.toSorted((a, b) => b.name.localeCompare(a.name))) {
        product.order = order;
        order += 1000;
      }
    }
  }

  deleteCartProduct = (productName: string) => {
    this.cartProducts = this.cartProducts.filter(x => x.name !== productName);
  }

  showInfo = (message: string) => {
    this.modalState = { header: "Info", message: message };
  }

  showError = (message: string) => {
    this.modalState = { header: "Error", message: message };
  }

  checkError = (response: Error | null | undefined) => {
    if (response instanceof Error) {
      this.showError(response.message);
    }
  }

  clearModal = () => {
    this.modalState = { header: null, message: null };
  }
}

const store = new Store()

export default store




