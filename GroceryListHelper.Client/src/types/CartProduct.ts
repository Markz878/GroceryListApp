import type { StoreProduct } from "./StoreProducts";

export interface CartProduct extends StoreProduct {
    amount: number;
    order: number;
    isCollected: boolean;
}