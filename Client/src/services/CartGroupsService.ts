import type { CartGroup, CreateCartGroupRequest } from "../types/CartGroup";
import { get, send } from "../helpers/httpHelpers";

const url = "api/cartgroups/";

export async function getCartGroups() {
    const response = await get(url, "GET", async (r) => await r.json() as CartGroup[]);
    return response;
}

export async function getCartGroup(id: string) {
    const response = await get(url + id, "GET", async (r) => await r.json() as CartGroup);
    return response;
}

export async function createCartGroup(cartGroupRequest: CreateCartGroupRequest) {
    const response = await get(url, "POST", async (r) => (await r.text()).trim().replaceAll('"', ''), { name: cartGroupRequest.name, otherUsers: Array.from(cartGroupRequest.otherUsers) });
    return response;
}

export async function updateCartGroupName(id: string, name: string) {
    const response = await send(url + id, "PUT", { name });
    return response;
}

export async function deleteCartGroup(id: string) {
    const response = await send(url + id, "DELETE");
    return response;
}