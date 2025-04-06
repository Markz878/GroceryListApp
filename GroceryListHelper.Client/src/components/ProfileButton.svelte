<script lang="ts">
  import { onMount } from "svelte";
  import { EmailClaimName, NameClaimName } from "../helpers/globalConstants";
  import { link } from "svelte-spa-router";
  import type { UserInfo } from "../types/UserInfo";

  let { userInfo }: { userInfo: UserInfo } = $props();

  let email = $state<string>();
  let userName = $state<string>();

  onMount(() => {
    email = userInfo.claims.find((x) => x.type === EmailClaimName)?.value;
    userName = userInfo.claims.find((x) => x.type === NameClaimName)?.value;

    document.addEventListener("click", (event) => {
      const target = event.target as HTMLElement;
      const profileMenu = document.getElementById("profile-menu") as HTMLDivElement;
      const profileBtn = document.getElementById("profile-btn") as HTMLInputElement;

      if (profileMenu && !profileMenu.contains(target) && !profileBtn?.contains(target)) {
        profileBtn.checked = false;
      }
    });
  });

  function getUserInitials(userName: string, email: string) {
    if (userName) {
      const names = userName.toUpperCase().split(" ", 2);
      if (names.length === 2) {
        return (names[0]?.[0] ?? "") + (names[1]?.[0] ?? "");
      }
      return userName.substring(0, 2).toUpperCase();
    }
    if (email) {
      return email.substring(0, 2).toUpperCase();
    }
    return "U";
  }

  function closeMenu() {
    const checkBox = document.getElementById("profile-btn") as HTMLInputElement;
    checkBox.checked = false;
  }
</script>

<div class="relative z-20 select-none">
  {#if userName && email}
    <label for="profile-btn" class="block relative z-10 bg-green-500 border-2 text-center leading-8 border-green-600 w-9 h-9 rounded-full hover:border-black hover:dark:border-white" aria-label="profile-btn">{getUserInitials(userName, email)}</label>
    <input type="checkbox" id="profile-btn" class="appearance-none absolute top-0 w-9 h-9 rounded-full" />
  {/if}
  <div id="profile-menu" class="min-w-max hidden absolute rounded-lg right-0 p-3 mt-1 border-2 border-black bg-[#FEF9F3] dark:bg-gray-800 dark:border-white">
    {#if userName}
      <p class="flex items-center">
        <img src="icons/user.svg" alt="User" class="h-4 mr-2 dark:invert" aria-hidden="true" />
        {userName}
      </p>
    {/if}

    {#if email}
      <p class="flex text-sm items-center">
        <img src="icons/email.svg" alt="User" class="h-4 mr-2 dark:invert" aria-hidden="true" />
        {email}
      </p>
    {/if}
    <p class="flex items-center">
      <img class="h-4 mr-2 dark:invert" src="icons/group.svg" alt="Group" aria-hidden="true" />
      <a href="/managegroups" use:link class="hover:text-gray-400 hover:underline" onclick={closeMenu}>Manage groups</a>
    </p>
    <form id="signout-form" method="post" action="api/Account/Logout" class="flex items-center">
      <img src="icons/logout.svg" alt="Log out" class="h-4 mr-2 dark:invert" aria-hidden="true" />
      <button id="signout-btn" class="hover:text-gray-400 hover:underline" type="submit">Sign out</button>
    </form>
  </div>
</div>

<style>
  #profile-btn:checked ~ #profile-menu {
    display: block;
  }
</style>
