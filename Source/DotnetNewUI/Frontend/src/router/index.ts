import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";
import SearchView from "../views/SearchView.vue";

const routes: Array<RouteRecordRaw> = [
  {
    path: "/",
    name: "search",
    component: SearchView,
  },
  {
    path: "/installed",
    name: "installed",
    component: () => import("../views/InstalledView.vue"),
  },
  {
    path: "/create",
    name: "create",
    component: () => import("../views/CreateView.vue"),
  },
  {
    path: "/create/:id",
    name: "create-item",
    component: () => import("../views/CreateItemView.vue"),
  },
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
});

export default router;
