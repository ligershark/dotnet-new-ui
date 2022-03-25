<template>
  <footer class="footer">
    <span class="footer__copyright"
      >Copyright © {{ new Date().getFullYear() }}
      <a href="https://rehansaeed.com">Muhammad Rehan Saeed</a> &
      <a href="https://github.com/petroemil">Emil Petro</a></span
    >
    <span>|</span>
    <span class="footer__links"
      >Built with <a href="https://dot.net">.NET</a></span
    >
    <span>|</span>
    <span class="footer__links"
      >Code on
      <a href="https://github.com/ligershark/dotnet-new-ui">GitHub</a></span
    >
  </footer>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "ui-footer",
});
</script>

<style lang="scss">
.footer {
  display: flex;
  justify-content: center;
  gap: 20px;

  padding: 20px;
}

.footer a {
  color: #fff;
}
.footer a:hover {
  background: hsl(283, 69%, 41%);
  transform: scale(1.1);
}

@media print {
  .footer {
    display: none;
  }
}
</style>
