import * as signalR from "@microsoft/signalr";
import * as signalRMsgPack from "@microsoft/signalr-protocol-msgpack"
import { addCartProduct, showInfo, updateCartProduct, deleteCartProduct, deleteCartProducts, isSharing, sortCartProducts, sortState } from "./store";
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
  showInfo(message);
});

connection.on("ProductAdded", (product: PascalCase<CartProduct>) => {
  addCartProduct({ amount: product.Amount, isCollected: product.IsCollected, name: product.Name, order: product.Order, unitPrice: product.UnitPrice });
});

connection.on("ProductModified", (product: PascalCase<CartProduct>) => {
  updateCartProduct({ amount: product.Amount, isCollected: product.IsCollected, name: product.Name, order: product.Order, unitPrice: product.UnitPrice });
});

connection.on("ProductDeleted", (name: string) => {
  deleteCartProduct(name);
});

connection.on("ProductsDeleted", () => {
  deleteCartProducts();
});

connection.on("ProductsSorted", (direction: SortDirection) => {
  sortCartProducts(direction);
  sortState.set(direction);
});

export async function joinGroup(groupId: string) {
  try {
    if (connection.state === "Disconnected") {
      await connection.start();
      await connection.invoke("JoinGroup", groupId);
      isSharing.set(true);
    }
  } catch (e) {
    console.log(e);
  }
}

export async function leaveGroup(groupId: string) {
  try {

    await connection.invoke("LeaveGroup", groupId);
    await connection.stop();
    isSharing.set(false);
  } catch (e) {
    console.log(e);
  }
}

export function getConnectionId() {
  return connection.connectionId ?? "";
}