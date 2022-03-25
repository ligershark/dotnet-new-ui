import "./assets/style/index.scss";

import { createApp } from "vue";
import { createMetaManager } from "vue-meta";
import App from "./App.vue";
import router from "./router";
// import { getOriginUrl } from "@/composables/Templates";

// window.onbeforeunload = function () {
//   const origin = getOriginUrl();
//   fetch(`${origin}/Shutdown`, { method: "POST", keepalive: true });
// };

createApp(App).use(createMetaManager()).use(router).mount("#app");
