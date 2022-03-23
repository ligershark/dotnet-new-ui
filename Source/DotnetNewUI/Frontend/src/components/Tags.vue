<template>
  <ul v-if="tags.length > 0" class="tags">
    <li v-for="tag in tags" v-bind:key="tag" class="tags__tag">
      <a
        class="tags__tag-link"
        :href="`https://www.nuget.org/packages?q=Tags:%22${tag}%22`"
        >🏷️ {{ tag }}</a
      >
    </li>
  </ul>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "ui-card",
  props: {
    tags: {
      type: Object as () => Array<string>,
      required: true,
    },
  },
});
</script>

<style lang="scss">
.tags {
  display: flex;
  align-items: baseline;
  column-gap: 20px;
  flex-wrap: wrap;

  list-style: none;
}

.tags__tag-link {
  color: hsl(0, 0%, 100%);
  text-decoration: none;
}
.tags__tag-link:hover {
  text-decoration: underline;
}
</style>
