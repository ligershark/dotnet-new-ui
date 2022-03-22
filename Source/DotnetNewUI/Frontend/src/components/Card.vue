<template>
  <div class="card"><slot></slot></div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "ui-card",
});
</script>

<style type="scss">
.card {
  display: grid;

  background: hsl(257, 85%, 15%);
  border: 1px solid hsl(0, 0%, 78%);
  border-radius: 4px;
  border-image-slice: 1;
  border-image-source: linear-gradient(
    121.26deg,
    hsl(312, 100%, 42%) -24.03%,
    hsl(253, 66%, 50%) 85.5%
  );
  color: hsl(0, 0, 100%);
  padding: 20px;
}
.card:hover {
  border: 1px solid hsl(253, 66%, 50%);
  background-image: linear-gradient(
    133.5deg,
    hsla(312, 100%, 42%, 0) 22.55%,
    hsla(254, 77%, 58%, 0.51) 72.74%
  );
}
</style>
