import type { CartProduct } from "./CartProduct";
import type { SortDirection } from "./SortState";

export interface ICartProductsService {
    getCartProducts: () => Promise<CartProduct[] | Error>;
    createCartProduct: (product: CartProduct) => Promise<Error | null>;
    updateCartProduct: (product: CartProduct) => Promise<Error | null>;
    sortCartProducts: (sortDirection: SortDirection) => Promise<Error | null>;
    deleteCartProduct: (name: string) => Promise<Error | null>;
    deleteAllCartProducts: () => Promise<Error | null>;
}