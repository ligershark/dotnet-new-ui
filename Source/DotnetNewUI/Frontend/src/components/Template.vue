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
      <h2 class="template__title-heading">{{ templateManifest.name }}</h2>
      <p class="template__type">{{ templateManifest.tags.type }}</p>
    </div>
    <p>{{ templateManifest.description }}</p>
    <ui-tags class="template__tags" :tags="tags" />
  </div>
</template>

<script lang="ts">
import { computed, defineComponent, toRefs } from "vue";
import ITemplate from "@/models/ITemplate";

export default defineComponent({
  name: "ui-template",
  props: {
    template: {
      type: Object as () => ITemplate,
      required: true,
    },
  },
  setup(props) {
    const { template } = toRefs(props);
    const iconUrl = computed(() => template.value.base64Icon);
    const tags = computed(() =>
      [template.value.templateManifest.tags.language].concat(
        template.value.templateManifest.classifications
      )
    );
    return {
      ...template,
      iconUrl,
      tags,
    };
  },
});
</script>
