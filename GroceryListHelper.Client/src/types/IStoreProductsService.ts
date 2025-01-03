import type { StoreProduct } from "./StoreProduct";

export interface IStoreProductsService {
    getStoreProducts: () => Promise<StoreProduct[] | Error>;
    createStoreProduct: (product: StoreProduct) => Promise<Error | null>;
    updateStoreProduct: (product: StoreProduct) => Promise<Error | null>;
    deleteStoreProducts: () => Promise<Error | null>;
}