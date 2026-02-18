<template>
  <div class="layout">
    <!-- Top Bar -->
    <header class="topbar">
      <div class="logo">Fitness Tracker</div>
      <nav class="nav-links">
        <router-link to="/" exact>Dashboard</router-link>
        <router-link to="/workouts">Workouts</router-link>
        <router-link to="/workouts/new">New Workout</router-link>
        <router-link to="/exercises">Exercises</router-link>
        <router-link to="/login">Login</router-link>
      </nav>
    </header>

    <!-- Main Content -->
    <main class="content">
      <slot />
    </main>
  </div>
</template>

<script setup lang="ts"></script>

<style scoped>
.layout {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  font-family: sans-serif;
}

.topbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: #1f2937;
  color: white;
  padding: 15px 30px;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}

.logo {
  font-size: 24px;
  font-weight: bold;
}

.nav-links a {
  margin-left: 25px;
  text-decoration: none;
  color: white;
  font-size: 16px;
}

.nav-links a.router-link-exact-active {
  font-weight: bold;
  color: #3b82f6;
}

.content {
  flex: 1;
  padding: 30px;
  background: #f3f4f6;
}
</style>
