<script lang="ts">
  let dialog = $state<HTMLDialogElement>();

  let { header, message, confirm, cancel }: { header: string; message: string; confirm: () => void; cancel?: () => void } = $props();

  export function showModal() {
    dialog?.showModal();
  }
</script>

<dialog class="border-2 border-gray-500 mt-32 mx-auto rounded max-w-md transition-[top] duration-300 starting:-top-60 p-0 backdrop:bg-gray-900/60" bind:this={dialog}>
  <div class="font-semibold">
    <div class="bg-[#28a745] text-white p-2">
      <h3 class="text-xl font-semibold">{header}</h3>
    </div>
    <div class="p-2 dark:bg-slate-900 dark:text-white">
      <p class="pb-2">{message}</p>
      <form method="dialog" class="flex justify-around">
        <button class="btn btn-success" onclick={confirm} aria-label="Yes">Yes</button>
        <button class="btn btn-danger" onclick={cancel} aria-label="No">No</button>
      </form>
    </div>
  </div>
</dialog>
