import type { IStoreProductsService } from "../types/IStoreProductsService";
import type { UserInfo } from "../types/UserInfo";
import { StoreProductsApiService } from "./StoreProductsApiService";
import { StoreProductsLocalService } from "./StoreProductsLocalService";

export function getStoreProductsService(userInfo?: UserInfo): IStoreProductsService {
    if (userInfo?.isAuthenticated) {
        return new StoreProductsApiService();
    }
    else {
        return new StoreProductsLocalService();
    }
}