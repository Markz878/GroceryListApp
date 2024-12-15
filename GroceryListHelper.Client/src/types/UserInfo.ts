import type { ClaimValue } from "./ClaimValue";

export interface UserInfo {
  isAuthenticated: false;
  claims: ClaimValue[];
}