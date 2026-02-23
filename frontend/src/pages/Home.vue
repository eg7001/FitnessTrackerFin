<template>
  <TopLayout>
    <div class="home">
      <h1>Welcome to Fitness Tracker</h1>
      <p>Track your workouts, manage exercises, and monitor your progress.</p>

      <div class="actions">
        <router-link to="/dashboard" class="btn primary">Go to Dashboard</router-link>
        <router-link to="/workouts" class="btn">View Workouts</router-link>
      </div>
    </div>
  </TopLayout>
</template>

<script setup lang="ts">
import TopLayout from '../components/TopLayout.vue'
</script>

<style scoped>
.home {
  max-width: 700px;
}

.actions {
  margin-top: 20px;
  display: flex;
  gap: 10px;
}

.btn {
  padding: 10px 16px;
  background: #e5e7eb;
  border-radius: 6px;
  text-decoration: none;
  color: #111827;
  font-weight: 500;
}

.btn.primary {
  background: #2563eb;
  color: white;
}
</style>
