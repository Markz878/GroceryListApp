import type { StoreProduct } from "../types/StoreProducts";
import type { IStoreProductsService } from "../types/IStoreProductsService";
import { storeProductsKey } from "../helpers/globalConstants";
import { storeProducts } from "../helpers/store";
import { get } from 'svelte/store';

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
            localStorage.setItem(storeProductsKey, JSON.stringify(get(storeProducts)));
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