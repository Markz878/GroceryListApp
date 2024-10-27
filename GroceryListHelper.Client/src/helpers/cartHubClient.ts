import * as signalR from "@microsoft/signalr";
import * as signalRMsgPack from "@microsoft/signalr-protocol-msgpack"
import store from "./store.svelte";
import type { SortDirection } from "../types/SortState";
import type { CartProduct } from "../types/CartProduct";
import type { PascalCase } from "../types/PascalCase";


const connection = new signalR.HubConnectionBuilder()
  .withUrl(import.meta.env.DEV ? "https://localhost:7021/carthub" : "/carthub")
  .configureLogging(import.meta.env.DEV ? signalR.LogLevel.Information : signalR.LogLevel.Warning)
  .withHubProtocol(new signalRMsgPack.MessagePackHubProtocol())
  .withAutomaticReconnect()
  .withStatefulReconnect()
  .build();

connection.on("GetMessage", (message: string) => {
  console.log(message);
  store.showInfo(message);
});

connection.on("ProductAdded", (product: PascalCase<CartProduct>) => {
  store.cartProducts.push({ amount: product.Amount, isCollected: product.IsCollected, name: product.Name, order: product.Order, unitPrice: product.UnitPrice });
});

connection.on("ProductModified", (product: PascalCase<CartProduct>) => {
  store.updateCartProduct({ amount: product.Amount, isCollected: product.IsCollected, name: product.Name, order: product.Order, unitPrice: product.UnitPrice });
});

connection.on("ProductDeleted", (name: string) => {
  store.deleteCartProduct(name);
});

connection.on("ProductsDeleted", () => {
  store.cartProducts = []
});

connection.on("ProductsSorted", (direction: SortDirection) => {
  store.sortCartProducts(direction);
  store.sortState = direction;
});

export async function joinGroup(groupId: string) {
  try {
    if (connection.state === "Disconnected") {
      await connection.start();
      await connection.invoke("JoinGroup", groupId);
      store.isSharing = true;
    }
  } catch (e) {
    console.log(e);
  }
}

export async function leaveGroup(groupId: string) {
  try {

    await connection.invoke("LeaveGroup", groupId);
    await connection.stop();
    store.isSharing = false;
  } catch (e) {
    console.log(e);
  }
}

export function getConnectionId() {
  return connection.connectionId ?? "";
}