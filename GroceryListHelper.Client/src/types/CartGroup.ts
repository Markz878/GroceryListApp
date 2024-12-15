import { SvelteSet } from "svelte/reactivity";

export interface CartGroup {
    id: string;
    name: string;
    otherUsers: SvelteSet<string>;
}

export interface CreateCartGroupRequest {
    name: string;
    otherUsers: SvelteSet<string>;
}