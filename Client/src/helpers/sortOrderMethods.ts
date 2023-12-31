export function getNewOrder(orders: number[], movingOrder: number, movingToOrder: number) {
    let result = -1;
    const minOrder = Math.min(...orders);
    const maxOrder = Math.max(...orders);
    if (movingToOrder === minOrder) {
        result = minOrder / 2;
    }
    else if (movingToOrder === maxOrder) {
        result = maxOrder + 1000;
    }
    else if (movingOrder < movingToOrder) {
        const nextLargestOrder = getNextLargerOrder(orders.filter(x => x !== movingOrder), movingToOrder);
        result = (nextLargestOrder + movingToOrder) / 2;
    }
    else if (movingOrder > movingToOrder) {
        const nextLargestOrder = getNextSmallerOrder(orders.filter(x => x != movingOrder), movingToOrder);
        result = (nextLargestOrder + movingToOrder) / 2;
    }
    return result;
}

function getNextLargerOrder(orders: number[], movingToOrder: number) {
    let result = Number.MAX_VALUE;
    for (const x of orders) {
        if (x > movingToOrder && x < result) {
            result = x;
        }
    }
    return result;
}

function getNextSmallerOrder(orders: number[], movingToOrder: number) {
    let result = Number.MIN_VALUE;
    for (const x of orders) {
        if (x < movingToOrder && x > result) {
            result = x;
        }
    }
    return result;
}