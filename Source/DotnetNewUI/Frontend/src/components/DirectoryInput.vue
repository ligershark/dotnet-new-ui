<template>
  <div class="directory-input">
    <input
      class="directory-input__input"
      :id="id"
      type="file"
      @change="onChangeLocation"
      webkitdirectory
      mozdirectory
      msdirectory
      odirectory
      directory />
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "ui-directory-input",
  props: {
    id: {
      type: String,
    },
  },
  setup(props, { emit }) {
    function onChangeLocation(event: Event) {
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      emit("update:modelValue", (event.target as any)?.files[0]);
    }

    return {
      onChangeLocation,
    };
  },
});
</script>

<style lang="scss">
.directory-input {
  display: grid;
  grid-template-columns: 1fr auto;

  position: relative;
}

.directory-input__input {
  cursor: pointer;
  opacity: 0;
}

.directory-input::after {
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
