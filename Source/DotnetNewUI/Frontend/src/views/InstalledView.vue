<template>
  <div class="installed">
    <h1>Installed</h1>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useMeta } from "vue-meta";

export default defineComponent({
  name: "InstalledView",
  setup() {
    useMeta({
      title: "Installed",
    });
  },
});
</script>
