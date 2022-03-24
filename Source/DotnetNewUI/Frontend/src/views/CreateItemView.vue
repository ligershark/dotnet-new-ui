<template>
  <div class="create-item">
    <h1>Create</h1>
    <form class="create-item__content" @submit.prevent="onSubmit">
      <fieldset class="create-item__fieldset">
        <label class="create-item__label" for="name">Name</label>
        <input id="name" type="text" />
      </fieldset>

      <fieldset class="create-item__fieldset">
        <label class="create-item__label" for="location">Directory</label>
        <div class="create-item__location-wrapper">
          <input
            id="location"
            type="file"
            webkitdirectory
            mozdirectory
            msdirectory
            odirectory
            directory />
        </div>
      </fieldset>

      <ui-button type="submit">🚀 Create</ui-button>
    </form>
  </div>
</template>

<script lang="ts">
import { defineComponent, onMounted, ref } from "vue";
import { useRoute } from "vue-router";
import Button from "@/components/Button.vue";
import { useTemplates } from "@/composables/Templates";
import ITemplate from "@/models/ITemplate";

export default defineComponent({
  name: "ui-create-item",
  components: {
    "ui-button": Button,
  },
  setup() {
    const route = useRoute();
    const templateId = route.params.id;
    const template = ref<ITemplate | null>(null);

    onMounted(async () => {
      const { data, error } = await useTemplates();
      if (data.value) {
        template.value = data.value.find(
          (x) => x.templateManifest.identity === templateId
        );
      } else if (error.value) {
        console.error(error.value);
      }
    });

    function onSubmit() {
      alert("test");
    }

    return {
      template,
      onSubmit,
    };
  },
});
</script>

<style lang="scss">
.create-item {
  display: grid;
  gap: 20px;
  justify-content: center;

  padding: 20px;
}

.create-item__content {
  display: grid;

  gap: 20px;

  max-width: 50rem;
}

.create-item__fieldset {
  display: flex;
  gap: 10px;
  flex-direction: column;
}

.create-item__location-wrapper {
  input {
    cursor: pointer;
    opacity: 0;
  }
  position: relative;
}
.create-item__location-wrapper::after {
  content: "Browse";

  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: 1;

  background: hsl(0, 0%, 100%);
  border-radius: 4px;
  color: hsl(283, 69%, 41%);
  font-size: 30px;
  line-height: 1.5;
  min-width: 160px;
  padding: 0.5rem 1rem;
  pointer-events: none;
  text-align: center;
  text-decoration: none;
  transition: transform 0.15s ease-in-out;
}
</style>
