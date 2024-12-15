import { get } from "../helpers/httpHelpers";
import type { UserInfo } from "../types/UserInfo";

let cachedAuthState: UserInfo | null = null;

export const getAuthenticationStateAsync = async (): Promise<UserInfo> => {
  if (cachedAuthState) {
    return cachedAuthState;
  }
  const response = await get("api/account/user", "GET", async (r) => await r.json() as UserInfo);
  if (response instanceof Error) {
    return { isAuthenticated: false, claims: [] }
  }
  cachedAuthState = response;
  return cachedAuthState;
}

export const forceAuthenticationAsync = async () => {
  const response = await get("api/account/user", "GET", async (r) => await r.json() as UserInfo);
  if (response instanceof Error || response.isAuthenticated === false) {
    window.location.href = "/api/Account/Login";
    return null;
  }
  else {
    cachedAuthState = response;
    return cachedAuthState;
  }
}