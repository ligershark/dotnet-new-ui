<template>
  <button class="button"><slot></slot></button>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "ui-button",
});
</script>

<style type="scss">
.button {
  display: inline-block;

  background: hsl(0, 0%, 100%);
  border-radius: 4px;
  color: hsl(283, 69%, 41%);

  cursor: pointer;
  font-size: 1.25rem;
  line-height: 1.5;
  min-width: 180px;
  padding: 0.5rem 1rem;
  text-align: center;
  text-decoration: none;
  transition: transform 0.15s ease-in-out;
}
.button:hover {
  transform: scale(1.1);
}
.button--active {
  background: hsla(257, 85%, 15%, 0.25);
  color: hsl(0, 0%, 100%);
  transform: scale(1.1);
}
</style>
