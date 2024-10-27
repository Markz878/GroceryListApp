import type { ICartProductsService } from "../types/ICartProductsService";
import { CartProductsApiService } from "./CartProductsApiService";
import { CartProductsLocalService } from "./CartProductsLocalService";
import { CartProductsGroupService } from "./CartProductsGroupService";
import type { UserInfo } from "../types/UserInfo";

export function getCartProductsService(userInfo?: UserInfo): ICartProductsService {
  if (userInfo?.isAuthenticated) {
    if (window.location.href.includes("groupcart")) {
      return new CartProductsGroupService();
    }
    else {
      return new CartProductsApiService();
    }
  }
  else {
    return new CartProductsLocalService();
  }
}