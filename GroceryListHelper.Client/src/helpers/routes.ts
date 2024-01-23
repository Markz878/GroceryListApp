import Home from "../pages/Home.svelte"
import GroupCart from "../pages/GroupCart.svelte"
import ManageGroups from "../pages/ManageGroups.svelte"

export const routes = {
    "/": Home,
    "/groupcart/:groupid": GroupCart,
    "/managegroups": ManageGroups
  };
