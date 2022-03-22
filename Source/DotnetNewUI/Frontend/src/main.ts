import "./assets/style/index.scss";

import { createApp } from "vue";
import { createMetaManager } from "vue-meta";
import App from "./App.vue";
import router from "./router";

createApp(App).use(createMetaManager()).use(router).mount("#app");
