<script lang="ts">
  import { onMount } from "svelte";
  import type { CreateCartGroupRequest, CartGroup } from "../types/CartGroup";
  import { forceAuthenticationAsync } from "../services/AuthenticationStateProvider";
  import { createCartGroup, deleteCartGroup, getCartGroups, updateCartGroupName } from "../services/CartGroupsService";
  import store from "../helpers/store.svelte";
  import Confirm from "../components/Confirm.svelte";
  import { link } from "svelte-spa-router";
  import { SvelteSet } from "svelte/reactivity";

  let cartGroups = $state<CartGroup[]>();
  let isCreatingNewGroup = $state(false);
  let createCartGroupRequest = $state<CreateCartGroupRequest>({ name: "", otherUsers: new SvelteSet<string>() });
  let newMemberEmail = $state("");
  let isBusy = $state(false);
  let oldGroupName = "";
  let editingGroup = $state<CartGroup | null>(null);
  let deletingGroup = $state<CartGroup | null>(null);
  let confirm = $state<Confirm>();

  onMount(async () => {
    const userInfo = await forceAuthenticationAsync();
    if (userInfo?.isAuthenticated) {
      const groupsResponse = await getCartGroups();
      if (groupsResponse instanceof Error) {
        store.showError("Could not load groups, please try again later");
        console.log(groupsResponse.message);
      } else {
        cartGroups = groupsResponse;
      }
    }
  });

  function removeNewMember(email: string) {
    createCartGroupRequest.otherUsers.delete(email);
    createCartGroupRequest = createCartGroupRequest;
  }

  function addMember() {
    createCartGroupRequest.otherUsers.add(newMemberEmail);
    newMemberEmail = "";
    createCartGroupRequest = createCartGroupRequest;
  }

  async function createGroup() {
    if (isCreatingNewGroup) {
      isBusy = true;
      try {
        const response = await createCartGroup(createCartGroupRequest);
        if (response instanceof Error) {
          store.showError(response.message);
        } else {
          const cartGroup: CartGroup = { id: "", name: "", otherUsers: new SvelteSet<string>() };
          cartGroup.id = response;
          cartGroup.name = createCartGroupRequest.name;
          cartGroup.otherUsers = createCartGroupRequest.otherUsers;
          cartGroups?.push(cartGroup);
          cartGroups = cartGroups;
          isCreatingNewGroup = false;
          createCartGroupRequest = { name: "", otherUsers: new SvelteSet<string>() };
        }
      } catch (e) {
        if (e instanceof Error) {
          store.showError(e.message);
        }
      } finally {
        isBusy = false;
      }
    } else {
      isCreatingNewGroup = true;
    }
  }

  function cancelGroupDelete() {
    deletingGroup = null;
  }

  function editGroup(group: CartGroup) {
    oldGroupName = group.name;
    editingGroup = group;
  }

  function showDeleteConfirm(group: CartGroup) {
    deletingGroup = group;
    if (confirm) {
      confirm.showModal();
    }
  }

  async function submitEditGroup(group: CartGroup) {
    editingGroup = null;
    if (group.name !== oldGroupName) {
      try {
        const response = await updateCartGroupName(group.id, group.name);
        if (response instanceof Error) {
          store.showError(response.message);
        }
      } catch (e) {
        if (e instanceof Error) {
          store.showError(e.message);
        }
      }
    }
    oldGroupName = "";
  }

  function stopEditGroup() {
    editingGroup = null;
    oldGroupName = "";
  }

  async function deleteGroup() {
    if (deletingGroup) {
      try {
        const response = await deleteCartGroup(deletingGroup.id);
        if (response instanceof Error) {
          store.showError(response.message);
        } else {
          cartGroups = cartGroups?.filter((x) => x.id !== deletingGroup?.id);
          deletingGroup = null;
        }
      } catch (e) {
        if (e instanceof Error) {
          store.showError(e.message);
        }
      }
    }
  }
</script>

<div class="p-4">
  {#if isCreatingNewGroup}
    <fieldset class="mt-4 p-4 border-2 border-gray-300 rounded-lg">
      <legend class="text-lg font-bold px-1">Create cart group</legend>
      <input id="create-group-name" class="form-control" bind:value={createCartGroupRequest.name} maxlength="30" size="30" placeholder="Group name" />
      <div class="flex flex-wrap mr-2 mb-2 px-[5px] py-1 items-center">
        <p class="mr-2">Members:</p>
        {#each createCartGroupRequest.otherUsers as email}
          <div class="flex mx-3 my-1">
            <span class="align-middle">{email}</span>
            <button class="btn btn-danger w-6 h-6 p-0 leading-4 ml-1 font-bold" onclick={() => removeNewMember(email)}>&times;</button>
          </div>
        {/each}
      </div>
      <div class="flex">
        <input id="add-user-to-group" class="form-control" bind:value={newMemberEmail} size="30" placeholder="New member email" />
        <button id="add-member-btn" class="btn btn-success w-9 h-9 p-0 ml-2 font-extrabold text-2xl" onclick={addMember}>+</button>
      </div>
      <button id="create-group-btn" class="btn btn-success flex items-center mt-4 gap-2" onclick={createGroup}>
        <img src="icons/group.svg" alt="group" class="m-auto h-5 invert" />
        {#if isBusy}
          <div class="flex items-center">
            <span class="border-4 border-[rgba(0,0,0,0.1)] border-t-white rounded-[50%] w-4 h-4 mr-2 animate-spin"></span>
            <span>Creating...</span>
          </div>
        {:else}
          <span>Create Group</span>
        {/if}
      </button>
    </fieldset>
  {:else}
    <button id="create-group-btn" class="btn btn-success flex items-center gap-2" onclick={createGroup}>
      <img src="icons/group.svg" alt="group" class="m-auto h-5 invert" />
      Create group
    </button>
  {/if}
  {#if !cartGroups || cartGroups.length === 0}
    <p>You have no groups yet.</p>
  {:else}
    {#each cartGroups as group}
      <fieldset class="flex flex-wrap items-center gap-3 mt-4 p-4 border-2 border-gray-300 rounded-lg">
        {#if editingGroup !== group}
          <div class="flex items-center gap-1">
            <a href="/groupcart/{group.id}" use:link class="btn btn-primary inline-flex gap-2">
              <svg class="w-4 invert" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512">
                <path d="M152.1 38.2c9.9 8.9 10.7 24 1.8 33.9l-72 80c-4.4 4.9-10.6 7.8-17.2 7.9s-12.9-2.4-17.6-7L7 113C-2.3 103.6-2.3 88.4 7 79s24.6-9.4 33.9 0l22.1 22.1 55.1-61.2c8.9-9.9 24-10.7 33.9-1.8zm0 160c9.9 8.9 10.7 24 1.8 33.9l-72 80c-4.4 4.9-10.6 7.8-17.2 7.9s-12.9-2.4-17.6-7L7 273c-9.4-9.4-9.4-24.6 0-33.9s24.6-9.4 33.9 0l22.1 22.1 55.1-61.2c8.9-9.9 24-10.7 33.9-1.8zM224 96c0-17.7 14.3-32 32-32H480c17.7 0 32 14.3 32 32s-14.3 32-32 32H256c-17.7 0-32-14.3-32-32zm0 160c0-17.7 14.3-32 32-32H480c17.7 0 32 14.3 32 32s-14.3 32-32 32H256c-17.7 0-32-14.3-32-32zM160 416c0-17.7 14.3-32 32-32H480c17.7 0 32 14.3 32 32s-14.3 32-32 32H192c-17.7 0-32-14.3-32-32zM48 368a48 48 0 1 1 0 96 48 48 0 1 1 0-96z" />
              </svg>
              <span>{group.name}</span>
            </a>
            <button class="btn btn-success h-8 w-8 p-[3px]" onclick={() => editGroup(group)} aria-label="Edit group name" disabled={editingGroup !== null}>
              <img src="icons/edit.svg" alt="Edit" class="m-auto invert" />
            </button>
            <button class="btn btn-danger p-[4px] w-8 h-8 text-lg leading-4" onclick={() => showDeleteConfirm(group)} aria-label="Delete group">
              <img src="icons/delete.svg" alt="Delete" class="m-auto invert" />
            </button>
          </div>
        {:else}
          <div class="flex items-center gap-1">
            <input id="edit-group-name-input" class="form-control-small" aria-label="Group name input" bind:value={group.name} />
            <button class="btn btn-success h-8 w-8 p-0" aria-label="Submit name edit" onclick={() => submitEditGroup(group)}>
              <img src="icons/check.svg" alt="Accept" class="p-[2px] invert" aria-hidden="true" />
            </button>
            <button class="btn btn-danger px-1 leading-4 h-8" aria-label="Cancel name edit" onclick={stopEditGroup}>Cancel</button>
          </div>
        {/if}
        <p>Members: You, {Array.from(group.otherUsers).join(", ")}</p>
      </fieldset>
    {/each}
  {/if}
</div>
<Confirm header="Confirm delete" message="This will delete the group and its products for all group members, are you sure?" on:ok={deleteGroup} on:cancel={cancelGroupDelete} bind:this={confirm} />
