import type { CartProduct } from "../types/CartProduct";
import type { ICartProductsService } from "../types/ICartProductsService";
import type { SortDirection } from "../types/SortState";
import { cartProductsKey } from "../helpers/globalConstants";
import store from "../helpers/store.svelte";

export class CartProductsLocalService implements ICartProductsService {
    getCartProducts = async () => {
        try {
            const productsString = localStorage.getItem(cartProductsKey);
            const products = productsString ? JSON.parse(productsString) as CartProduct[] : [];
            return products;
        }
        catch (e) {
            if (e instanceof Error) {
                return e;
            }
            else {
                return new Error("Unknow problem loading from local storage");
            }
        }
    }

    deleteAllCartProducts = () => {
        return this.saveCartProductsToLocalStorage();
    }

    createCartProduct = (product: CartProduct) => {
        return this.saveCartProductsToLocalStorage();
    }

    updateCartProduct = (product: CartProduct) => {
        return this.saveCartProductsToLocalStorage();
    }

    sortCartProducts = (sortDirection: SortDirection) => {
        return this.saveCartProductsToLocalStorage();
    }

    deleteCartProduct = (name: string) => {
        return this.saveCartProductsToLocalStorage();
    }

    private async saveCartProductsToLocalStorage() {
        try {
            localStorage.setItem(cartProductsKey, JSON.stringify(store.cartProducts));
            return null;
        }
        catch (e) {
            if (e instanceof Error) {
                return e;
            }
            else {
                return new Error("Unknow problem saving to local storage");
            }
        }
    }
}