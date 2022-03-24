<template>
  <div class="create-item">
    <h1>Create</h1>
    <form class="create-item__content" @submit.prevent="onSubmit">
      <fieldset v-if="languages.length > 1" class="create-item__fieldset">
        <label class="create-item__label" for="language">Language</label>
        <select class="create-item__label" v-model="language" id="language">
          <option
            v-for="language in languages"
            v-bind:key="language"
            :value="language">
            {{ language }}
          </option>
        </select>
      </fieldset>

      <fieldset class="create-item__fieldset">
        <label class="create-item__label" for="name">Name</label>
        <input id="name" type="text" v-model="name" />
      </fieldset>

      <fieldset class="create-item__fieldset">
        <label class="create-item__label" for="location">Directory</label>
        <input id="name" type="text" v-model="location" />
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
    const name = ref("");
    const location = ref("");
    const languages = ref<Array<string>>([]);
    const language = ref("");

    onMounted(async () => {
      const { data, error } = await useTemplates();
      if (data.value) {
        template.value =
          data.value.find((x) => x.templateManifest.identity === templateId) ||
          null;
        languages.value = [...new Set(template.value?.languages)];
      } else if (error.value) {
        console.error(error.value);
      }
    });

    function onChangeLocation(event: Event) {
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      location.value = (event.target as any)?.files[0];
    }

    function onSubmit() {
      alert("test");
    }

    return {
      template,
      name,
      location,
      languages,
      language,
      onChangeLocation,
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

  min-width: 50rem;
}

.create-item__fieldset {
  display: flex;
  gap: 10px;
  flex-direction: column;
}
</style>
