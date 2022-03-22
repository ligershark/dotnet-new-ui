import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";
import SearchView from "../views/SearchView.vue";
import InstalledView from "../views/InstalledView.vue";
import CreateView from "../views/CreateView.vue";

const routes: Array<RouteRecordRaw> = [
  {
    path: "/",
    name: "search",
    component: SearchView,
  },
  {
    path: "/installed",
    name: "installed",
    component: InstalledView,
  },
  {
    path: "/create",
    name: "create",
    component: CreateView,
  },
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
});

export default router;
