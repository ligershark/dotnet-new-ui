<template>
  <div class="installed">
    <h1>Installed</h1>
    <div class="installed__templates">
      <ui-card v-for="template in templates" v-bind:key="template.id">
        <ui-template :template="template" />
      </ui-card>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, onMounted, ref } from "vue";
import { useMeta } from "vue-meta";
import Card from "@/components/Card.vue";
import Template from "@/components/Template.vue";
import { useInstalled } from "@/composables/Templates";
import ITemplate from "@/models/ITemplate";

export default defineComponent({
  name: "InstalledView",
  components: {
    "ui-card": Card,
    "ui-template": Template,
  },
  setup() {
    useMeta({
      title: "Installed",
    });

    let templates = ref<ITemplate[] | null>(null);

    onMounted(async () => {
      const { data, error } = await useInstalled();
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
.installed {
  display: grid;
  gap: 20px;

  padding: 20px;
}

.installed__templates {
  display: grid;
  gap: 20px;
  justify-content: center;
}
</style>
