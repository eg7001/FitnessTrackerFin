<script lang="ts" setup>
import { ref, onMounted } from 'vue'
import type { Workout } from '@/types/workout'
import { getWorkouts } from '@/services/workoutService'

// Reactive state
const workouts = ref<Workout[]>([])
const loading = ref(false)
const error = ref<string | null>(null)

// Load workouts on mount
onMounted(async () => {
  loading.value = true
  error.value = null

  try {
    workouts.value = await getWorkouts() // <- must assign to .value
    console.log('Fetched workouts:', workouts.value) // debug
  } catch (err: any) {
    error.value = err.message || 'Failed to load workouts'
  } finally {
    loading.value = false
  }
})

// Optional: format date for display
function formatDate(dateStr: string): string {
  const d = new Date(dateStr)
  return d.toLocaleDateString() + ' ' + d.toLocaleTimeString()
}
</script>

<template>
  <div class="workouts-page">
    <h1>Workouts</h1>

    <!-- Loading state -->
    <p v-if="loading">Loading workouts...</p>

    <!-- Error state -->
    <p v-if="error" class="error">{{ error }}</p>

    <!-- Empty state -->
    <p v-if="!loading && workouts.length === 0">No workouts found.</p>

    <!-- Workouts list -->
    <ul v-if="!loading && workouts.length > 0">
      <li v-for="w in workouts" :key="w.id">
        <router-link :to="{ name: 'WorkoutDetail', params: { id: w.id } }">
          <strong>{{ w.name }}</strong> — {{ formatDate(w.date) }}
        </router-link>
      </li>
    </ul>
  </div>
</template>

<style scoped>
.workouts-page {
  max-width: 600px;
  margin: 0 auto;
  padding: 1rem;
}

.error {
  color: red;
  font-weight: bold;
}
</style>
