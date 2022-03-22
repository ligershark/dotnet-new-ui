<template>
  <nav class="navigation">
    <router-link
      class="navigation__link"
      active-class="navigation__link--active"
      to="/"
      >🔎 Search</router-link
    >
    <router-link
      class="navigation__link"
      active-class="navigation__link--active"
      to="/installed"
      >💽 Installed</router-link
    >
    <router-link
      class="navigation__link"
      active-class="navigation__link--active"
      to="/create"
      >🚀 Create</router-link
    >
  </nav>
</template>

<style type="scss">
.navigation {
  display: flex;
  flex-wrap: wrap;
  gap: 20px;
  justify-content: center;

  padding: 20px;
}

.navigation__link {
  display: inline-block;

  background: hsla(257, 85%, 15%, 0.25);
  border-radius: 4px;
  color: hsl(0, 0%, 100%);
  cursor: pointer;
  font-size: 1.25rem;
  line-height: 1.5;
  min-width: 180px;
  padding: 0.5rem 1rem;
  text-align: center;
  text-decoration: none;
  transition: transform 0.15s ease-in-out;
}
.navigation__link:hover {
  transform: scale(1.1);
}
.navigation__link--active {
  background: hsl(0, 0%, 100%);
  color: #8821b2;
  transform: scale(1.1);
}
</style>
