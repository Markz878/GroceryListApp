import type { CartProduct } from "../types/CartProduct";
import type { ICartProductsService } from "../types/ICartProductsService";
import type { SortDirection } from "../types/SortState";
import { send } from "../helpers/httpHelpers";

export class CartProductsApiService implements ICartProductsService {
    readonly url = "api/cartproducts/";

    getCartProducts = async () => {
        const productsResponse = await fetch(this.url);
        const products = await productsResponse.json() as CartProduct[];
        return products;
    }

    createCartProduct = async (product: CartProduct) => {
        const response = await send(this.url, "POST", product);
        return response;
    }

    updateCartProduct = async (product: CartProduct) => {
        const response = await send(this.url, "POST", product);
        return response;
    }

    sortCartProducts = async (sortDirection: SortDirection) => {
        const url = this.url + "sort/" + (sortDirection === "Ascending" ? "0" : "1");
        const response = await send(url, "PATCH");
        return response;
    }

    deleteAllCartProducts = async () => {
        const response = await send(this.url, "DELETE");
        return response;
    }

    deleteCartProduct = async (name: string) => {
        const response = await send(this.url + name, "DELETE");
        return response;
    }
}