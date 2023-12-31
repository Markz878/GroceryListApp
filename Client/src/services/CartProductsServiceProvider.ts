import type { ICartProductsService } from "../types/ICartProductsService";
import { CartProductsApiService } from "./CartProductsApiService";
import { CartProductsLocalService } from "./CartProductsLocalService";
import { getAuthenticationStateAsync } from "./AuthenticationStateProvider";
import { CartProductsGroupService } from "./CartProductsGroupService";

export async function getCartProductsService(): Promise<ICartProductsService> {
    const userInfo = await getAuthenticationStateAsync();
    if (userInfo.isAuthenticated) {
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