<template>
  <article class="template">
    <img
      v-if="template.iconUrl"
      class="template__icon"
      width="256"
      height="256"
      :src="template.iconUrl"
    />
    <div class="template__title">
      <h2 class="template__title-heading">
        <a class="template__title-heading-link" :href="projectUrl"
          >{{ title }}
          <span
            v-if="verified"
            class="template__title-verified"
            title="The ID prefix of this package has been reserved for one of the owners of this package by NuGet.org."
            >✅</span
          ></a
        >
      </h2>
      <p class="template__title-authors">
        by
        <span v-for="author in authors" v-bind:key="author">{{ author }}</span>
      </p>
    </div>
    <div class="template__statistics">
      <p class="template__downloads">
        ⬇️ {{ totalDownloads.toLocaleString("en-US") }} downloads
      </p>
      <p class="template__last-updated">⏳ last updated</p>
      <p class="template__version">🚀 Latest version: {{ version }}</p>
    </div>
    <ul v-if="tags.length > 0" class="template__tags">
      <li v-for="tag in tags" v-bind:key="tag" class="template__tag">
        <a
          class="template__tag-link"
          :href="`https://www.nuget.org/packages?q=Tags:%22${tag}%22`"
          >🏷️ {{ tag }}</a
        >
      </li>
    </ul>
    <p class="template__description">{{ description }}</p>
    <ui-button class="template__install" @click="onInstallClick"
      >💽 Install</ui-button
    >
  </article>
</template>

<script lang="ts">
import { defineComponent, toRefs } from "vue";
import Button from "@/components/Button.vue";
import ITemplate from "@/models/ITemplate";

export default defineComponent({
  name: "ui-template",
  components: {
    "ui-button": Button,
  },
  props: {
    template: {
      type: Object as () => ITemplate,
      required: true,
    },
  },
  setup(props) {
    let { template } = toRefs(props);

    function onInstallClick(event: PointerEvent) {
      console.log("Install", event);
    }

    return {
      onInstallClick,
      ...template.value,
    };
  },
});
</script>

<style type="scss">
.template {
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

.template__icon {
  grid-area: icon;

  width: 100px;
  height: 100px;
}

.template__title {
  grid-area: title;

  display: flex;
  align-items: baseline;
  gap: 10px;
  flex-wrap: wrap;
}
.template__title-heading-link {
  color: hsl(0, 0%, 100%);
  text-decoration: none;
}
.template__title-heading-link:hover {
  text-decoration: underline;
}

.template__statistics {
  grid-area: statistics;

  display: flex;
  align-items: baseline;
  gap: 20px;
  flex-wrap: wrap;
}

.template__tags {
  grid-area: tags;

  display: flex;
  align-items: baseline;
  column-gap: 20px;
  flex-wrap: wrap;

  list-style: none;
}
.template__tag-link {
  color: hsl(0, 0%, 100%);
  text-decoration: none;
}
.template__tag-link:hover {
  text-decoration: underline;
}

.template__description {
  grid-area: description;

  overflow-wrap: break-word;
  word-wrap: break-word;
  word-break: break-word;
}

.template__install {
  grid-area: install;

  align-self: start;
}
</style>
