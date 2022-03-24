<template>
  <router-link class="anchor" :to="to"><slot></slot></router-link>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "ui-button",
  props: {
    to: {
      type: Object,
    },
  },
});
</script>

<style type="scss">
.anchor {
  display: inline-block;

  background: hsl(0, 0%, 100%);
  border-radius: 4px;
  color: hsl(283, 69%, 41%);
  cursor: pointer;
  font-size: 1.25rem;
  line-height: 1.5;
  min-width: 160px;
  padding: 0.5rem 1rem;
  text-align: center;
  text-decoration: none;
  transition: transform 0.15s ease-in-out;
}
.anchor:hover {
  transform: scale(1.1);
}
.anchor--active {
  background: hsla(257, 85%, 15%, 0.25);
  color: hsl(0, 0%, 100%);
  transform: scale(1.1);
}
</style>
