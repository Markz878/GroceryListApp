import type { IStoreProductsService } from "../types/IStoreProductsService";
import type { StoreProduct } from "../types/StoreProduct";
import { getAuthenticationStateAsync } from "./AuthenticationStateProvider";
import { StoreProductsApiService } from "./StoreProductsApiService";
import { StoreProductsLocalService } from "./StoreProductsLocalService";

export class StoreProductsProxyService implements IStoreProductsService {
  localService = new StoreProductsLocalService();
  apiService = new StoreProductsApiService();

  getService = async (): Promise<IStoreProductsService> => {
    const userInfo = await getAuthenticationStateAsync();
    if (userInfo?.isAuthenticated) {
      return this.apiService;
    }
    else {
      return this.localService;
    }
  }

  getStoreProducts = async () => {
    const service = await this.getService();
    return await service.getStoreProducts();
  }
  createStoreProduct = async (product: StoreProduct) => {
    const service = await this.getService();
    return await service.createStoreProduct(product);
  }
  updateStoreProduct = async (product: StoreProduct) => {
    const service = await this.getService();
    return await service.updateStoreProduct(product);
  }
  deleteStoreProducts = async () => {
    const service = await this.getService();
    return await service.deleteStoreProducts();
  }
}