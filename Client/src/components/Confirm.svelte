<script lang="ts">
  import { createEventDispatcher } from "svelte";

  let dialog: HTMLDialogElement | null;

  export let header: string;
  export let message: string;

  const dispatch = createEventDispatcher();

  function confirm() {
    dispatch("ok");
  }

  function cancel() {
    dispatch("cancel");
  }

  export function showModal() {
    dialog?.showModal();
  }
</script>

<dialog class="border-2 border-gray-500 mt-32 rounded max-w-md animate-fade-in-down p-0 backdrop:bg-gray-900/60" bind:this={dialog}>
  <div class="font-semibold">
    <div class="bg-[#28a745] text-white p-2">
      <h3 class="text-xl font-semibold">{header}</h3>
    </div>
    <div class="p-2 dark:bg-slate-900 dark:text-white">
      <p class="pb-2">{message}</p>
      <form method="dialog" class="flex justify-around">
        <button class="btn btn-success" on:click={confirm} aria-label="Yes">Yes</button>
        <button class="btn btn-danger" on:click={cancel} aria-label="No">No</button>
      </form>
    </div>
  </div>
</dialog>
