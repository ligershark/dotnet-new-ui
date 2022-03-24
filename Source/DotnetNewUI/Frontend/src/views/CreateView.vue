<template>
  <div class="create">
    <h1>Create New Project</h1>
    <div class="create__filters">
      <input
        class="create__query-input"
        type="search"
        placeholder="Template search e.g. Boxed"
        v-model="query" />
      <select class="create__type-input" v-model="type">
        <option value="">All templates</option>
        <option value="item">🟩 Item template</option>
        <option value="project">🟦 Project template</option>
        <option value="solution">🟥 Solution template</option>
      </select>
      <select class="create__language-input" v-model="language">
        <option value="">All languages</option>
        <option
          v-for="language in languages"
          v-bind:key="language"
          :value="language">
          {{ language }}
        </option>
      </select>
      <select class="create__project-type-input" v-model="projectType">
        <option value="">All project types</option>
        <option
          v-for="projectType in projectTypes"
          v-bind:key="projectType"
          :value="projectType">
          {{ projectType }}
        </option>
      </select>
    </div>
    <div class="create__templates">
      <ui-card
        class="create__template"
        v-for="template in filteredTemplates"
        v-bind:key="
          template.templateManifest.identity +
          template.templateManifest?.tags?.language
        ">
        <ui-template :template="template" />
      </ui-card>
    </div>
  </div>
</template>

<script lang="ts">
import { computed, defineComponent, onMounted, ref } from "vue";
import { useMeta } from "vue-meta";
import Card from "@/components/Card.vue";
import Template from "@/components/Template.vue";
import { useTemplates } from "@/composables/Templates";
import ITemplate from "@/models/ITemplate";

export default defineComponent({
  name: "CreateView",
  components: {
    "ui-card": Card,
    "ui-template": Template,
  },
  setup() {
    useMeta({
      title: "Create",
    });

    let templates = ref<ITemplate[] | null>(null);
    const query = ref("");
    const type = ref("");
    const languages = ref<Array<string>>([]);
    const language = ref("");
    const projectTypes = ref<Array<string>>([]);
    const projectType = ref("");

    var filteredTemplates = computed(() => {
      return templates.value?.filter((x) => {
        console.log(
          x.templateManifest.name,
          query.value,
          type.value,
          language.value
        );
        return (
          (type.value === "" ||
            x.templateManifest?.tags?.type === type.value) &&
          (language.value === "" ||
            x.templateManifest?.tags?.language === language.value) &&
          (projectType.value === "" ||
            x.templateManifest?.classifications.includes(projectType.value)) &&
          x.templateManifest.name
            .toLowerCase()
            .includes(query.value.toLowerCase())
        );
      });
    });

    onMounted(async () => {
      const { data, error } = await useTemplates();
      if (data.value) {
        languages.value = [
          ...new Set(
            data.value
              .map((x) => x.templateManifest?.tags?.language)
              .filter((x) => x)
          ),
        ];
        projectTypes.value = [
          ...new Set(
            data.value
              .flatMap((x) => x.templateManifest?.classifications)
              .filter((x) => x)
          ),
        ].sort();
        templates.value = data.value.sort((a, b) => {
          const fa = a.templateManifest.name,
            fb = b.templateManifest.name;

          if (fa < fb) {
            return -1;
          }
          if (fa > fb) {
            return 1;
          }
          return 0;
        });
      } else if (error.value) {
        console.error(error.value);
      }
    });

    return {
      filteredTemplates,
      query,
      type,
      languages,
      language,
      projectTypes,
      projectType,
    };
  },
});
</script>

<style type="scss">
.create {
  display: grid;
  gap: 20px;

  padding: 20px;
}

.create__filters {
  display: grid;
  gap: 20px;
  grid-template-areas:
    "query"
    "type"
    "language"
    "project-type";
  grid-template-columns: 1fr;
}
.create__query-input {
  grid-area: query;
}
.create__type-input {
  grid-area: type;
}
.create__language-input {
  grid-area: language;
}
.create__project-type-input {
  grid-area: project-type;
}

@media screen and (min-width: 750px) {
  .create__filters {
    grid-template-areas:
      "query query"
      "type language"
      "project-type .";
    grid-template-columns: repeat(2, 1fr);
  }
}
@media screen and (min-width: 1000px) {
  .create__filters {
    grid-template-areas:
      "query query query"
      "type language project-type";
    grid-template-columns: repeat(3, 1fr);
  }
}

.create__templates {
  display: grid;
  gap: 20px;
}
</style>
