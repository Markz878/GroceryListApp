<script lang="ts">
  import Router, { link } from "svelte-spa-router";
  import { routes } from "./helpers/routes";
  import Modal from "./components/Modal.svelte";
  import { onMount } from "svelte";
  import { getAuthenticationStateAsync } from "./services/AuthenticationStateProvider";
  import ProfileButton from "./components/ProfileButton.svelte";
  import store from "./helpers/store.svelte";

  onMount(async () => {
    const authState = await getAuthenticationStateAsync();
    store.authInfo = authState;
  });
</script>

<nav class="flex justify-center bg-gray-200 w-full dark:bg-gray-500 dark:text-white">
  <div class="flex justify-between w-full max-w-content px-4 items-center">
    <div class="flex items-center select-none">
      <a href="/" use:link class="flex items-center">
        <img width="48" height="48" src="/cart128.png" alt="Logo" />
        <span class="font-bold text-xl dark:text-gray-100">Grocery List Helper</span>
      </a>
    </div>
    {#if store.authInfo?.isAuthenticated}
      <ProfileButton userInfo={store.authInfo} />
    {:else}
      <a id="login-link" class="btn btn-success font-bold text-lg mx-4 align-middle" href="api/Account/Login">Log in</a>
    {/if}
  </div>
</nav>

<main class="min-h-full bg-[url('../store-transparent.webp')] bg-cover">
  <div class="bg-gray-100 mx-auto max-w-content dark:bg-gray-800 dark:text-white">
    <Router {routes} />
  </div>
</main>
<Modal />
