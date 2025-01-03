import type { StoreProduct } from "./StoreProduct";

export interface CartProduct extends StoreProduct {
    amount: number;
    order: number;
    isCollected: boolean;
}