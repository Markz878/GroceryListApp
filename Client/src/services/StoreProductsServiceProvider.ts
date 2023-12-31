import type { IStoreProductsService } from "../types/IStoreProductsService";
import { getAuthenticationStateAsync } from "./AuthenticationStateProvider";
import { StoreProductsApiService } from "./StoreProductsApiService";
import { StoreProductsLocalService } from "./StoreProductsLocalService";

export async function getStoreProductsService(): Promise<IStoreProductsService> {
    const userInfo = await getAuthenticationStateAsync();
    if (userInfo.isAuthenticated) {
        return new StoreProductsApiService();
    }
    else {
        return new StoreProductsLocalService();
    }
}