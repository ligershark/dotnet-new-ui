import "./assets/style/index.scss";

import { createApp } from "vue";
import { createMetaManager } from "vue-meta";
import App from "./App.vue";
import router from "./router";

window.onbeforeunload = function () {
  fetch("/Shutdown", { method: "POST", keepalive: true });
};

createApp(App).use(createMetaManager()).use(router).mount("#app");
