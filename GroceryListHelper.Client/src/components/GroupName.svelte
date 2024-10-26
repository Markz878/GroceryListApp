<script lang="ts">
  import type { CartGroup } from "../types/CartGroup";
  import { isSharing } from "../helpers/store";
  interface Props {
    groupInfo: CartGroup;
  }

  let { groupInfo }: Props = $props();
</script>

<div class="pt-4 pl-4">
  <div class="flex items-center">
    <h3 class="text-2xl">{groupInfo.name}</h3>
    <div class="relative w-10 h-3">
      <p class="mx-4 w-3 h-3 absolute rounded-full bg-green-500 animate-ping motion-reduce:animate-none {$isSharing ? '' : 'hidden'}"></p>
      <p class="mx-4 w-3 h-3 absolute rounded-full {$isSharing ? 'bg-green-500' : 'bg-red-500'}"></p>
    </div>
    <p class="text-sm text-gray-400">{$isSharing ? "Sharing cart" : "Sharing starting..."}</p>
  </div>
  <p class="my-2"><b>Members: </b>{Array.from(groupInfo.otherUsers).join(", ")}</p>
</div>
