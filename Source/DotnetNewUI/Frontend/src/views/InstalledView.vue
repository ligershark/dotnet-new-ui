<template>
  <div class="installed">
    <h1>Installed</h1>
    <div class="installed__packages">
      <ui-card v-for="pack in packages" v-bind:key="pack.id">
        <ui-package :pack="pack" />
      </ui-card>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, onMounted, ref } from "vue";
import { useMeta } from "vue-meta";
import Card from "@/components/Card.vue";
import Package from "@/components/Package.vue";
import { useInstalled } from "@/composables/Templates";
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

    onMounted(async () => {
      const { data, error } = await useInstalled();
      console.log(data.value, error.value);
      if (data.value) {
        packages.value = data.value;
      } else if (error.value) {
        console.error(error.value);
      }
    });

    return {
      packages,
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
  justify-content: center;
}
</style>
