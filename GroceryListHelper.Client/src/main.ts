import "./app.pcss";
import App from "./App.svelte";


const appDiv = document.getElementById("app")!;

if (appDiv && appDiv.hasChildNodes()) {
  appDiv.innerHTML = '';
}
const app = new App({
  target: appDiv,
});

export default app;
