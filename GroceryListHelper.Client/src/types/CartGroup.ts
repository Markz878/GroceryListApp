import { SvelteSet } from "svelte/reactivity";

export class CartGroup {
    id = "";
    name = "";
    otherUsers = new SvelteSet<string>();
}

export class CreateCartGroupRequest {
    name = "";
    otherUsers = new SvelteSet<string>();
}