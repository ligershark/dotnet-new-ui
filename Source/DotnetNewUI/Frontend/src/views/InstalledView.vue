<template>
  <div class="installed">
    <h1>Installed</h1>
    <input
      type="search"
      placeholder="Template package search e.g. Boxed.Templates"
      v-model="query" />
    <div class="installed__packages">
      <ui-card v-for="pack in filteredPackages" v-bind:key="pack.id">
        <ui-package :pack="pack" />
      </ui-card>
    </div>
  </div>
</template>

<script lang="ts">
import { computed, defineComponent, onMounted, ref } from "vue";
import { useMeta } from "vue-meta";
import Card from "@/components/Card.vue";
import Package from "@/components/Package.vue";
import { usePackages } from "@/composables/Templates";
import IPackage from "@/models/IPackage";

export default defineComponent({
  name: "InstalledView",
  components: {
    "ui-card": Card,
    "ui-package": Package,
  },
  setup() {
    useMeta({
      title: "Installed",
    });

    let packages = ref<IPackage[] | null>(null);
    const query = ref("");

    var filteredPackages = computed(() => {
      if (query.value === "") {
        return packages.value?.slice(0, 100);
      }
      return packages.value
        ?.filter(
          (x) =>
            x.id.toLowerCase().includes(query.value.toLowerCase()) ||
            x.title.toLowerCase().includes(query.value.toLowerCase())
        )
        .slice(0, 100);
    });

    onMounted(async () => {
      const { data, error } = await usePackages();
      if (data.value) {
        packages.value = data.value.filter((x) => x.isInstalled);
      } else if (error.value) {
        console.error(error.value);
      }
    });

    return {
      filteredPackages,
      query,
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

.installed__packages {
  display: grid;
  gap: 20px;
}
</style>
