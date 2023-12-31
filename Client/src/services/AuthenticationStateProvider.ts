import { get } from "../helpers/httpHelpers";
import { UserInfo } from "../types/UserInfo";

let cachedAuthState: UserInfo | null = null;

export const getAuthenticationStateAsync = async () => {
    if (cachedAuthState) {
        return cachedAuthState;
    }
    const response = await get("api/account/user", "GET", async (r) => await r.json() as UserInfo);
    if (response instanceof Error) {
        return new UserInfo();
    }
    cachedAuthState = response;
    return cachedAuthState;
}