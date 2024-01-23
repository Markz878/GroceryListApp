import { getConnectionId, joinGroup } from "../helpers/cartHubClient";
import { get, send } from "../helpers/httpHelpers";
import type { CartProduct } from "../types/CartProduct";
import type { ICartProductsService } from "../types/ICartProductsService";
import type { SortDirection } from "../types/SortState";

export class CartProductsGroupService implements ICartProductsService {
    readonly url = "api/cartgroupproducts/";

    getCartProducts = async () => {
        await joinGroup(this.getGroupId());
        const productsResponse = await get(this.url + this.getGroupId(), "GET", async (r) => await r.json() as CartProduct[]);
        return productsResponse;
    }

    createCartProduct = async (product: CartProduct) => {
        await joinGroup(this.getGroupId());
        const response = await send(this.url + this.getGroupId(), "POST", product, { 'ConnectionId': getConnectionId() });
        return response;
    }

    updateCartProduct = async (product: CartProduct) => {
        await joinGroup(this.getGroupId());
        const response = await send(this.url + this.getGroupId(), "PUT", product, { 'ConnectionId': getConnectionId() });
        return response;
    }

    sortCartProducts = async (sortDirection: SortDirection) => {
        await joinGroup(this.getGroupId());
        const response = await send(this.url + this.getGroupId() + "/sort/" + (sortDirection === "Ascending" ? "0" : "1"), "PATCH", undefined, { 'ConnectionId': getConnectionId() });
        return response;
    }

    deleteAllCartProducts = async () => {
        await joinGroup(this.getGroupId());
        const response = await send(this.url + this.getGroupId(), "DELETE", undefined, { 'ConnectionId': getConnectionId() });
        return response;
    }

    deleteCartProduct = async (name: string) => {
        await joinGroup(this.getGroupId());
        const response = await send(this.url + this.getGroupId() + "/" + name, "DELETE", undefined, { 'ConnectionId': getConnectionId() });
        return response;
    }

    getGroupId = () => {
        const lastSlashIndex = window.location.href.lastIndexOf('/');
        const result = window.location.href.substring(lastSlashIndex + 1);
        return result;
    }
}