<template>
  <button class="button" :type="type"><slot></slot></button>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "ui-button",
  props: {
    type: {
      type: String as () => "submit" | "reset" | "button",
    },
  },
});
</script>

<style type="scss">
.button {
  display: inline-block;

  background: transparent;
  border: 1px solid hsl(0, 0%, 100%);
  border-radius: 4px;
  color: hsl(0, 0%, 100%);
  cursor: pointer;
  font-size: 1.25rem;
  line-height: 1.5;
  min-width: 160px;
  padding: 0.5rem 1rem;
  text-align: center;
  text-decoration: none;
  transition: transform 0.15s ease-in-out;
}
.button:hover {
  background: hsl(283, 69%, 41%);
  transform: scale(1.1);
}
.button[disabled] {
  cursor: not-allowed;
  opacity: 0.7;
}
</style>
