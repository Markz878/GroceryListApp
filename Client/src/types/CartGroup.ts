export class CartGroup {
    id = "";
    name = "";
    otherUsers = new Set<string>();
}

export class CreateCartGroupRequest {
    name = "";
    otherUsers = new Set<string>();
}