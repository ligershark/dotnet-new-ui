<template>
  <article class="package">
    <img
      class="package__icon"
      alt=""
      loading="lazy"
      :src="iconWithFallbackUrl"
      width="256"
      height="256" />
    <div class="package__title">
      <h2 class="package__title-heading">
        <a class="package__title-heading-link" :href="projectUrl"
          >{{ title }}
          <span
            v-if="verified"
            class="package__title-verified"
            title="The ID prefix of this package has been reserved for one of the owners of this package by NuGet.org."
            >✅</span
          ></a
        >
      </h2>
      <p class="package__title-authors">
        by
        <span v-for="author in authors" v-bind:key="author">{{ author }}</span>
      </p>
    </div>
    <div class="package__statistics">
      <p class="package__downloads">
        ⬇️ {{ totalDownloads.toLocaleString("en-US") }} downloads
      </p>
      <p class="package__last-updated">⏳ last updated</p>
      <p class="package__version">🚀 Latest version: {{ version }}</p>
    </div>
    <ui-tags v-if="tags.length > 0" class="package__tags" :tags="tags" />
    <p class="package__description">{{ description }}</p>
    <ui-button
      v-if="isInstalled"
      class="package__uninstall"
      @click="onUninstallClick"
      >☠️ Uninstall</ui-button
    >
    <ui-button v-else class="package__install" @click="onInstallClick"
      >💽 Install</ui-button
    >
  </article>
</template>

<script lang="ts">
import { computed, defineComponent, toRefs } from "vue";
import Button from "@/components/Button.vue";
import Tags from "@/components/Tags.vue";
import {
  usePackageInstall,
  usePackageUninstall,
} from "@/composables/Templates";
import IPackage from "@/models/IPackage";

export default defineComponent({
  name: "ui-package",
  components: {
    "ui-button": Button,
    "ui-tags": Tags,
  },
  props: {
    pack: {
      type: Object as () => IPackage,
      required: true,
    },
  },
  setup(props) {
    let { pack } = toRefs(props);

    const iconWithFallbackUrl = computed(() => {
      return pack.value.iconUrl || "/static/images/default-package-icon.svg";
    });

    async function onInstallClick() {
      const { data, error } = await usePackageInstall(pack.value.id);
      if (data.value) {
        alert(`${pack.value.title} installed!`);
      } else if (error.value) {
        console.error(error.value);
      }
    }

    async function onUninstallClick() {
      const { data, error } = await usePackageUninstall(pack.value.id);
      if (data.value) {
        alert(`${pack.value.title} uninstalled!`);
      } else if (error.value) {
        console.error(error.value);
      }
    }

    return {
      onInstallClick,
      onUninstallClick,
      iconWithFallbackUrl,
      ...pack.value,
    };
  },
});
</script>

<style type="scss">
.package {
  display: grid;
  column-gap: 10px;
  row-gap: 4px;
  grid-template-areas:
    "icon title install"
    "icon statistics install"
    "icon tags install"
    "icon description install"
    "icon . install";
  grid-template-columns: auto 1fr auto;
  grid-template-rows: auto auto auto auto 1fr;
}

.package__icon {
  grid-area: icon;

  width: 80px;
  height: 80px;
}

.package__title {
  grid-area: title;

  display: flex;
  align-items: baseline;
  gap: 10px;
  flex-wrap: wrap;
}
.package__title-heading-link {
  color: hsl(0, 0%, 100%);
  text-decoration: none;
}
.package__title-heading-link:hover {
  text-decoration: underline;
}
.package__title-heading-link:not([href]):hover {
  text-decoration: none;
}

.package__statistics {
  grid-area: statistics;

  display: flex;
  align-items: baseline;
  gap: 20px;
  flex-wrap: wrap;
}

.package__tags {
  grid-area: tags;
}

.package__description {
  grid-area: description;

  overflow-wrap: break-word;
  word-wrap: break-word;
  word-break: break-word;
}

.package__install {
  grid-area: install;

  align-self: start;
}
</style>