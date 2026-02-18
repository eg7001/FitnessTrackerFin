<template>
  <TopLayout>
    <h1>Workouts</h1>
    <router-link to="/workouts/new" class="btn">Create New Workout</router-link>

    <ul class="workout-list">
      <li v-for="w in workouts" :key="w.id">{{ w.name }} - {{ w.date }}</li>
    </ul>
  </TopLayout>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import TopLayout from '../components/TopLayout.vue'

interface Workout {
  id: number
  name: string
  date: string
}

const workouts = ref<Workout[]>([])

onMounted(() => {
  workouts.value = [
    { id: 1, name: 'Push Day', date: '2026-02-17' },
    { id: 2, name: 'Pull Day', date: '2026-02-16' },
    { id: 3, name: 'Leg Day', date: '2026-02-15' },
  ]
})
</script>

<style scoped>
.btn {
  display: inline-block;
  margin-bottom: 15px;
  padding: 10px 15px;
  background: #2563eb;
  color: white;
  border-radius: 6px;
  text-decoration: none;
}

.workout-list {
  list-style: none;
  padding: 0;
}

.workout-list li {
  background: white;
  padding: 15px;
  margin-bottom: 10px;
  border-radius: 6px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}
</style>
