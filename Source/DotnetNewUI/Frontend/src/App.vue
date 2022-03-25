<template>
  <metainfo>
    <template v-slot:title="{ content }">{{
      content ? `${content} | .NET New UI` : `.NET New UI`
    }}</template>
  </metainfo>
  <nprogress-container />
  <ui-navigation />
  <router-view class="app__router" />
  <ui-footer />
</template>

<script lang="ts">
import { defineComponent } from "vue";
import NprogressContainer from "vue-nprogress/src/NprogressContainer";
import Footer from "@/components/Footer.vue";
import Navigation from "@/components/Navigation.vue";

export default defineComponent({
  name: "App",
  components: {
    NprogressContainer,
    "ui-footer": Footer,
    "ui-navigation": Navigation,
  },
});
</script>
