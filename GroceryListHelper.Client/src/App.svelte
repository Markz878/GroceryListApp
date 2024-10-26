<script lang="ts">
  import Router, { link } from "svelte-spa-router";
  import { routes } from "./helpers/routes";
  import Modal from "./components/Modal.svelte";
  import { onMount } from "svelte";
  import { UserInfo } from "./types/UserInfo";
  import { getAuthenticationStateAsync } from "./services/AuthenticationStateProvider";
  import ProfileButton from "./components/ProfileButton.svelte";

  let userInfo = $state<UserInfo>();

  onMount(async () => {
    const authState = await getAuthenticationStateAsync();
    userInfo = authState;
  });
</script>

<nav class="flex justify-center bg-gray-200 w-full dark:bg-gray-500 dark:text-white">
  <div class="flex justify-between w-full max-w-[--content-max-width] px-4 items-center">
    <div class="flex items-center select-none">
      <a href="/" use:link class="flex items-center">
        <img width="48" height="48" src="/cart128.png" alt="Logo" />
        <span class="font-bold text-xl dark:text-gray-100">Grocery List Helper</span>
      </a>
    </div>
    {#if userInfo && userInfo.isAuthenticated}
      <ProfileButton />
    {:else}
      <a id="login-link" class="btn btn-success font-bold text-lg mx-4 align-middle" href="api/Account/Login">Log in</a>
    {/if}
  </div>
</nav>

<main class="min-h-full bg-[url('../store-transparent.webp')] bg-cover">
  <div class="bg-gray-100 mx-auto max-w-[--content-max-width] dark:bg-gray-800 dark:text-white">
    <Router {routes} />
    <Modal />
  </div>
</main>
