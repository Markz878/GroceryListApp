import type { StoreProduct } from "../types/StoreProduct";
import type { IStoreProductsService } from "../types/IStoreProductsService";
import { get, send } from "../helpers/httpHelpers";

export class StoreProductsApiService implements IStoreProductsService {
    readonly url = "api/storeproducts/";

    getStoreProducts = async () => {
        const productsResponse = await get(this.url, "GET", async (r) => await r.json() as StoreProduct[]);
        return productsResponse;
    }

    createStoreProduct = async (product: StoreProduct) => {
        const response = await send(this.url, "POST", product);
        return response;
    }

    updateStoreProduct = async (product: StoreProduct) => {
        const response = await send(this.url, "POST", product);
        return response;
    }

    deleteStoreProducts = async () => {
        const response = await send(this.url, "DELETE");
        return response;
    }
}