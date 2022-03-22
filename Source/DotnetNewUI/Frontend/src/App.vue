<template>
  <metainfo>
    <template v-slot:title="{ content }">{{
      content ? `${content} | .NET New UI` : `.NET New UI`
    }}</template>
  </metainfo>
  <ui-navigation />
  <router-view />
</template>

<script lang="ts">
import { defineComponent } from "vue";
import Navigation from "@/components/Navigation.vue"; // @ is an alias to /src

export default defineComponent({
  name: "App",
  components: {
    "ui-navigation": Navigation,
  },
});
</script>

<style lang="scss">
:root {
  background: hsl(257, 85%, 15%);
  color: white;
}

body {
  background-image: url("/public/static/images/anniversary-bg.png"),
    linear-gradient(90deg, #190649 95%, #a62dd8 5%);
  background-position-x: 38%;
}
</style>
