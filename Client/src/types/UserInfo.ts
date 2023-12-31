import type { ClaimValue } from "./ClaimValue";

export class UserInfo {
    isAuthenticated = false;
    claims: ClaimValue[] = [];
}