import type { CartProduct } from "../types/CartProduct";
import type { ICartProductsService } from "../types/ICartProductsService";
import type { SortDirection } from "../types/SortState";
import { getAuthenticationStateAsync } from "./AuthenticationStateProvider";
import { CartProductsApiService } from "./CartProductsApiService";
import { CartProductsGroupService } from "./CartProductsGroupService";
import { CartProductsLocalService } from "./CartProductsLocalService";

export class CartProductsProxyService implements ICartProductsService {
  localService = new CartProductsLocalService();
  apiService = new CartProductsApiService();
  groupService = new CartProductsGroupService();

  getService = async (): Promise<ICartProductsService> => {
    const userInfo = await getAuthenticationStateAsync();
    if (userInfo?.isAuthenticated) {
      if (window.location.href.includes("groupcart")) {
        return this.groupService;
      }
      else {
        return this.apiService;
      }
    }
    else {
      return this.localService;
    }
  }

  getCartProducts = async () => {
    const service = await this.getService();
    return await service.getCartProducts();
  }
  createCartProduct = async (product: CartProduct) => {
    const service = await this.getService();
    return await service.createCartProduct(product);
  }
  updateCartProduct = async (product: CartProduct) => {
    const service = await this.getService();
    return await service.updateCartProduct(product);
  }
  sortCartProducts = async (sortDirection: SortDirection) => {
    const service = await this.getService();
    return await service.sortCartProducts(sortDirection);
  }
  deleteCartProduct = async (name: string) => {
    const service = await this.getService();
    return await service.deleteCartProduct(name);
  }
  deleteAllCartProducts = async () => {
    const service = await this.getService();
    return await service.deleteAllCartProducts();
  }
}