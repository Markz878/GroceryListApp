import type { StoreProduct } from "../types/StoreProduct";
import type { IStoreProductsService } from "../types/IStoreProductsService";
import { storeProductsKey } from "../helpers/globalConstants";
import store from "../helpers/store.svelte";

export class StoreProductsLocalService implements IStoreProductsService {
    getStoreProducts = async () => {
        try {
            const productsString = localStorage.getItem(storeProductsKey);
            const products = productsString ? JSON.parse(productsString) as StoreProduct[] : [];
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

    deleteStoreProducts = () => {
        return this.saveStoreProductsToLocalStorage();
    }

    createStoreProduct = (product: StoreProduct) => {
        return this.saveStoreProductsToLocalStorage();
    }

    updateStoreProduct = (product: StoreProduct) => {
        return this.saveStoreProductsToLocalStorage();
    }

    private async saveStoreProductsToLocalStorage() {
        try {
            localStorage.setItem(storeProductsKey, JSON.stringify(store.storeProducts));
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