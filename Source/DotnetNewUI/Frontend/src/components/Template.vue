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
      <p
        v-if="templateManifest?.tags?.type"
        class="template__type"
        :class="`template__type--${templateManifest?.tags?.type}`">
        {{ templateManifest?.tags?.type }} template
      </p>
    </div>
    <ui-tags class="template__tags" :tags="tags" />
    <p class="template__description">{{ templateManifest.description }}</p>
  </div>
</template>

<script lang="ts">
import { computed, defineComponent, toRefs } from "vue";
import Tags from "@/components/Tags.vue";
import ITemplate from "@/models/ITemplate";

export default defineComponent({
  name: "ui-template",
  components: {
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
    const tags = computed(() => {
      const language = template.value?.templateManifest?.tags?.language;
      if (language) {
        return [language].concat(
          template.value.templateManifest.classifications
        );
      }
      return template.value.templateManifest.classifications;
    });

    return {
      ...template.value,
      iconUrl,
      tags,
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
    "icon title"
    "icon tags"
    "icon description"
    "icon .";
  grid-template-columns: auto 1fr;
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
.template__type {
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

.template__tags {
  grid-area: tags;
}

.template__description {
  grid-area: description;

  overflow-wrap: break-word;
  word-wrap: break-word;
  word-break: break-word;
}
</style>
