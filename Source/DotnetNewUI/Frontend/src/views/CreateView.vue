<template>
  <div class="create">
    <h1>Create</h1>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useMeta } from "vue-meta";

export default defineComponent({
  name: "CreateView",
  setup() {
    useMeta({
      title: "Create",
    });
  },
});
</script>
