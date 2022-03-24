<template>
  <div class="template">
    <img
      class="template__icon"
      alt=""
      loading="lazy"
      :src="iconUrl"
      width="256"
      height="256" />
    <div class="template__title">
      <h2 class="template__title-heading">
        <a
          class="template__title-heading-link"
          :href="ideHostManifest?.learnMoreLink"
          >{{ templateManifest.name }}</a
        >
      </h2>
      <p class="template__title-authors">
        by
        <span>{{ templateManifest?.author }}</span>
      </p>
      <p
        class="template__language"
        :class="`template__language--${getLanguageClass(language)}`"
        v-for="language in languages"
        v-bind:key="language">
        {{ language }}
      </p>
      <p
        v-if="templateManifest?.tags?.type"
        class="template__type"
        :class="`template__type--${templateManifest?.tags?.type}`">
        {{ templateManifest?.tags?.type }} template
      </p>
    </div>
    <ui-tags class="template__tags" :tags="templateManifest.classifications" />
    <p class="template__description">{{ templateManifest.description }}</p>
    <ui-anchor
      class="template__create"
      :to="{ name: 'create-item', params: { id: templateManifest.identity } }"
      >🚀 Create</ui-anchor
    >
  </div>
</template>

<script lang="ts">
import { computed, defineComponent, toRefs } from "vue";
import Anchor from "@/components/Anchor.vue";
import Tags from "@/components/Tags.vue";
import ITemplate from "@/models/ITemplate";

export default defineComponent({
  name: "ui-template",
  components: {
    "ui-anchor": Anchor,
    "ui-tags": Tags,
  },
  props: {
    template: {
      type: Object as () => ITemplate,
      required: true,
    },
  },
  setup(props) {
    const { template } = toRefs(props);

    const iconUrl = computed(() => template.value.base64Icon);

    function getLanguageClass(language: string) {
      return language.replace("#", "");
    }

    return {
      ...template.value,
      getLanguageClass,
      iconUrl,
    };
  },
});
</script>

<style lang="scss">
.template {
  display: grid;
  column-gap: 10px;
  row-gap: 4px;
  grid-template-areas:
    "icon title create"
    "icon tags create"
    "icon description create"
    "icon . create";
  grid-template-columns: auto 1fr auto;
  grid-template-rows: auto auto auto 1fr;
}

.template__icon {
  grid-area: icon;

  width: 80px;
  height: 80px;
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
.template__title-heading-link:not([href]):hover {
  text-decoration: none;
}
.template__language,
.template__type {
  background: coral;
  border-radius: 10px;
  padding: 2px 10px;
}
.template__type--item {
  background: green;
}
.template__type--project {
  background: blue;
}
.template__type--solution {
  background: red;
}
.template__language--C {
  background: purple;
}
.template__language--F {
  background: teal;
}
.template__language--VB {
  background: orange;
}

.template__tags {
  grid-area: tags;
}

.template__description {
  grid-area: description;

  overflow-wrap: break-word;
  word-wrap: break-word;
  word-break: break-word;
}

.template__create {
  grid-area: create;

  align-self: start;
}
</style>
