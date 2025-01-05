import App from "./App.svelte";
import { mount } from "svelte";
import './app.css'

const appDiv = document.getElementById("app")!;

if (appDiv && appDiv.hasChildNodes()) {
  appDiv.innerHTML = '';
}
const app = mount(App, {
  target: appDiv,
});

export default app;
