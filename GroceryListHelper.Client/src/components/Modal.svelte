<script lang="ts">
  import { modalState, clearModal } from "../helpers/store";

  let dialog = $state<HTMLDialogElement>();

  let modalBackground = $state("bg-green-600");

  modalState.subscribe((x) => {
    if (dialog && x.header !== null && x.message !== null) {
      setModalBackground();
      dialog.showModal();
    }
  });
  function closeModal() {
    clearModal();
  }

  function setModalBackground() {
    modalBackground = $modalState.header === "Error" ? "bg-red-600" : "bg-green-600";
  }
</script>

<dialog id="modal" class="border-2 border-gray-500 mt-32 rounded max-w-md animate-fade-in-down p-0 backdrop:bg-gray-900/60" bind:this={dialog}>
  <div class="font-semibold">
    <div class="flex justify-between items-center h-8 text-white p-2 {modalBackground}">
      <h3 class="text-xl font-semibold">{$modalState.header}</h3>
      <form method="dialog">
        <button class="text-white font-bold w-6 h-6 leading-5 bg-transparent" onclick={closeModal}>&times;</button>
      </form>
    </div>
    <div class="p-4 dark:bg-slate-900 dark:text-white">
      <p class="text-wrap break-words">{$modalState.message}</p>
    </div>
  </div>
</dialog>
