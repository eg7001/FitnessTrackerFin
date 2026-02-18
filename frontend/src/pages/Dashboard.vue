<template>
  <TopLayout>
    <h1>Dashboard</h1>

    <div class="stats">
      <div class="card">
        <h3>Total Workouts</h3>
        <p>{{ stats.workouts }}</p>
      </div>
      <div class="card">
        <h3>Total Exercises</h3>
        <p>{{ stats.exercises }}</p>
      </div>
      <div class="card">
        <h3>This Week</h3>
        <p>{{ stats.weekly }}</p>
      </div>
    </div>

    <div class="recent-workouts">
      <h2>Recent Workouts</h2>
      <ul>
        <li v-for="w in recent" :key="w.id">{{ w.name }} - {{ w.date }}</li>
      </ul>
    </div>
  </TopLayout>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import TopLayout from '../components/TopLayout.vue'

// Define types
interface Workout {
  id: number
  name: string
  date: string
}

// Reactive data
const stats = ref({ workouts: 0, exercises: 0, weekly: 0 })
const recent = ref<Workout[]>([])

onMounted(() => {
  stats.value = { workouts: 15, exercises: 42, weekly: 4 }
  recent.value = [
    { id: 1, name: 'Push Day', date: '2026-02-17' },
    { id: 2, name: 'Pull Day', date: '2026-02-16' },
    { id: 3, name: 'Leg Day', date: '2026-02-15' },
  ]
})
</script>

<style scoped>
.stats {
  display: flex;
  gap: 25px;
  margin: 20px 0;
}

.card {
  background: white;
  padding: 25px;
  border-radius: 8px;
  width: 200px;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
}

.card h3 {
  margin-bottom: 10px;
}

.card p {
  font-size: 28px;
  font-weight: bold;
}

.recent-workouts {
  margin-top: 40px;
}

.recent-workouts ul {
  list-style: none;
  padding: 0;
}

.recent-workouts li {
  background: white;
  padding: 15px;
  margin-bottom: 10px;
  border-radius: 6px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}
</style>
