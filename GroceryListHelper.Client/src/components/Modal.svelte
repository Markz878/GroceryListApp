<script lang="ts">
  import store from "../helpers/store.svelte";

  let dialog = $state<HTMLDialogElement>();

  $effect(() => {
    if (dialog && store.modalState.header !== null && store.modalState.message !== null) {
      dialog.showModal();
    }
  });

  function closeModal() {
    store.clearModal();
  }

  const modalBackground = $derived(store.modalState.header === "Error" ? "bg-red-600" : "bg-green-600");
</script>

<dialog id="modal" class="border-2 border-gray-500 rounded mx-auto mt-32 max-w-md transition-[top] duration-300 starting:-top-60 p-0 backdrop:bg-gray-900/60" bind:this={dialog}>
  <div class="font-semibold">
    <div class="flex justify-between items-center h-8 text-white p-2 {modalBackground}">
      <h3 class="text-xl font-semibold">{store.modalState.header}</h3>
      <form method="dialog">
        <button class="text-white font-bold w-6 h-6 leading-5 bg-transparent cursor-pointer" onclick={closeModal}>&times;</button>
      </form>
    </div>
    <div class="p-4 dark:bg-slate-900 dark:text-white">
      <p class="text-wrap break-words">{store.modalState.message}</p>
    </div>
  </div>
</dialog>
