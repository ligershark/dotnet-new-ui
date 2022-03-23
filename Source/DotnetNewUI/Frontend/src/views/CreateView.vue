<template>
  <div class="create">
    <h1>Create</h1>
    <div class="create__templates">
      <ui-template
        class="create__template"
        v-for="template in templates"
        v-bind:key="template.templateManifest.identity"
        :template="template" />
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, onMounted, ref } from "vue";
import { useMeta } from "vue-meta";
import Template from "@/components/Template.vue";
import { useTemplates } from "@/composables/Templates";
import ITemplate from "@/models/ITemplate";

export default defineComponent({
  name: "CreateView",
  components: {
    "ui-template": Template,
  },
  setup() {
    useMeta({
      title: "Create",
    });

    let templates = ref<ITemplate[] | null>(null);

    onMounted(async () => {
      const { data, error } = await useTemplates();
      console.log(data.value, error.value);
      if (data.value) {
        templates.value = data.value;
      } else if (error.value) {
        console.error(error.value);
      }
    });

    return {
      templates,
    };
  },
});
</script>

<style type="scss">
.create {
  display: grid;
  gap: 20px;

  padding: 20px;
}

.create__templates {
  display: grid;
  gap: 20px;
}
</style>
