<template>
  <div class="create">
    <h1>Create</h1>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useMeta } from "vue-meta";
import ITemplate from "@/models/ITemplate";

export default defineComponent({
  name: "CreateView",
  setup() {
    useMeta({
      title: "Create",
    });

    // let packages = ref<ITemplate[] | null>(null);

    // onMounted(async () => {
    //   const { data, error } = await useInstalled();
    //   console.log(data.value, error.value);
    //   if (data.value) {
    //     packages.value = data.value;
    //   } else if (error.value) {
    //     console.error(error.value);
    //   }
    // });

    // return {
    //   packages,
    // };
  },
});
</script>
