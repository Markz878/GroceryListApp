import { writable } from 'svelte/store';
import type { StoreProduct } from '../types/StoreProducts';
import type { CartProduct } from '../types/CartProduct';
import type { CartSortState, SortDirection } from '../types/SortState';
import type { ModalInfo } from '../types/ModalInfo';

export const cartProducts = writable<CartProduct[]>([]);
export const storeProducts = writable<StoreProduct[]>([]);
export const sortState = writable<CartSortState>("None");
export const showOnlyUncollected = writable<boolean>(false);
export const modalState = writable<ModalInfo>({ header: null, message: null })
export const isSharing = writable<boolean>(false);

export function addCartProduct(product: CartProduct) {
    cartProducts.update(x => [...x, product]);
}

export function updateCartProduct(product: CartProduct) {
    cartProducts.update(x => {
        const p = x.find(x => x.name === product.name);
        if (p) {
            p.amount = product.amount;
            p.isCollected = product.isCollected;
            p.unitPrice = product.unitPrice;
            if (p.order !== product.order) {
                p.order = product.order;
                sortState.set("None");
            }
        }
        return [...x];
    });
}

export function deleteCartProduct(productName: string) {
    cartProducts.update(x => x.filter(x => x.name !== productName));
}

export function deleteCartProducts() {
    cartProducts.set([]);
}

export function sortCartProducts(sortDirection: SortDirection) {
    let order = 1000;
    if (sortDirection === "Ascending") {
        cartProducts.update(x => {
            for (const product of x.toSorted((a, b) => a.name.localeCompare(b.name))) {
                product.order = order;
                order += 1000;
            }
            return [...x];
        });
    }
    else {
        cartProducts.update(x => {
            for (const product of x.toSorted((a, b) => b.name.localeCompare(a.name))) {
                product.order = order;
                order += 1000;
            }
            return [...x];
        });
    }
}

export function showInfo(message: string) {
    modalState.set({ header: "Info", message: message });
}

export function showError(message: string) {
    modalState.set({ header: "Error", message: message });
}

export function checkError(response: Error | null | undefined) {
    if (response instanceof Error) {
        showError(response.message);
    }
}

export function clearModal() {
    modalState.set({ header: null, message: null });
}

