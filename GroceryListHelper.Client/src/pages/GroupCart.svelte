<script lang="ts">
  import { onDestroy, onMount } from "svelte";
  import type { CartGroup } from "../types/CartGroup";
  import { getCartGroup } from "../services/CartGroupsService";
  import CartComponent from "../components/CartComponent.svelte";
  import CartSummaryRow from "../components/CartSummaryRow.svelte";
  import GroupName from "../components/GroupName.svelte";
  import { joinGroup, leaveGroup } from "../helpers/cartHubClient";
  import store from "../helpers/store.svelte";
  import { forceAuthenticationAsync } from "../services/AuthenticationStateProvider";

  interface Props {
    params?: { groupid: string };
  }

  let { params = { groupid: "" } }: Props = $props();

  let groupInfo = $state<CartGroup>();

  onMount(async () => {
    await forceAuthenticationAsync();
    const groupInfoResponse = await getCartGroup(params.groupid);
    if (groupInfoResponse instanceof Error) {
      store.showError(groupInfoResponse.message);
    } else {
      groupInfo = groupInfoResponse;
    }
    await joinGroup(params.groupid);
  });

  onDestroy(async () => {
    await leaveGroup(params.groupid);
  });
</script>

{#if groupInfo}
  <GroupName {groupInfo} />
{/if}
<CartComponent />
<CartSummaryRow />
