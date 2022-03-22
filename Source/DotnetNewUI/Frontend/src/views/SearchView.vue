<template>
  <div class="search">
    <h1 class="search__title">Search</h1>
    <input
      type="search"
      placeholder="Template package search e.g. Boxed.Templates"
    />
    <div class="search__templates">
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
import useFetch from "@/composables/Fetch";
import ITemplate from "@/models/ITemplate";

export default defineComponent({
  name: "SearchView",
  components: {
    "ui-card": Card,
    "ui-template": Template,
  },
  setup() {
    useMeta({
      title: "Search",
    });

    let templates = ref<ITemplate[] | null>(null);

    onMounted(async () => {
      const url = "http://localhost:4999/Templates/online";
      const { data, error } = await useFetch<Array<ITemplate>>(url);
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
.search {
  display: grid;
  gap: 20px;

  padding: 20px;
}

.search__templates {
  display: grid;
  gap: 20px;
  justify-content: center;
}
</style>
