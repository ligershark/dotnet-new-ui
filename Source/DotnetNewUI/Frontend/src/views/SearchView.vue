<template>
  <div class="search"><h1>Search</h1></div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useMeta } from "vue-meta";

export default defineComponent({
  name: "SearchView",
  setup() {
    useMeta({
      title: "Search",
    });
  },
});
</script>
