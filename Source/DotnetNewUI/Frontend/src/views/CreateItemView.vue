<template>
  <div class="create-item">
    <h1>Create</h1>
    <form class="create-item__content" @submit.prevent="onSubmit">
      <fieldset class="create-item__fieldset">
        <label class="create-item__label" for="location"
          >Output Directory
          <span class="create-item__label-required">*</span></label
        >
        <input id="name" type="text" v-model="location" />
      </fieldset>

      <fieldset v-if="languages.length > 1" class="create-item__fieldset">
        <label class="create-item__label" for="language"
          >Language <span class="create-item__label-required">*</span></label
        >
        <select class="create-item__label" v-model="language" id="language">
          <option
            v-for="language in languages"
            v-bind:key="language"
            :value="language">
            {{ language }}
          </option>
        </select>
      </fieldset>

      <fieldset v-if="!isItemTemplate" class="create-item__fieldset">
        <label class="create-item__label" for="name">Name</label>
        <input id="name" type="text" v-model="name" />
        <p class="create-item__note">
          The name for the output being created. If no name is specified, the
          name of the output directory is used.
        </p>
      </fieldset>

      <ui-button :disabled="!isValid" type="submit">🚀 Create</ui-button>
    </form>
  </div>
</template>

<script lang="ts">
import { computed, defineComponent, onMounted, ref } from "vue";
import { useRoute } from "vue-router";
import Button from "@/components/Button.vue";
import { useCreate, useTemplates } from "@/composables/Templates";
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
    const location = ref(
      navigator.appVersion?.indexOf("Win") !== -1 ? "C:\\" : "\\"
    );
    const languages = ref<Array<string>>([]);
    const language = ref("");

    const isItemTemplate = computed(
      () => template.value?.templateManifest?.tags?.type === "item"
    );
    const isValid = computed(
      () =>
        location.value !== "" &&
        (languages.value.length <= 1 || language.value !== "")
    );

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

    async function onSubmit() {
      if (template.value) {
        const { error } = await useCreate(
          template.value.templateManifest.shortName[0],
          location.value,
          name.value,
          language.value
        );
        if (error.value) {
          console.error(error.value);
        } else {
          alert("Created!");
        }
      }
    }

    return {
      template,
      isItemTemplate,
      name,
      location,
      languages,
      isValid,
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
.create-item__label-required {
  color: red;
}
.create-item__note {
  max-width: none;
}
</style>
