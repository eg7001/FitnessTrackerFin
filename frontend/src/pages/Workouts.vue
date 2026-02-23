<script lang="ts" setup>
// =============================
// Imports
// =============================
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import type { Workout } from '@/types/workout'
import { getWorkouts } from '@/services/workoutService'
import TopLayout from '@/components/TopLayout.vue'

// =============================
// Reactive State
// =============================
const workouts = ref<Workout[]>([])
const loading = ref(false)
const error = ref<string | null>(null)

// Router for navigation (optional, for programmatic navigation)
const router = useRouter()

// =============================
// Helper Functions
// =============================
const formatDate = (dateStr: string): string => {
  const d = new Date(dateStr)
  return d.toLocaleDateString() + ' ' + d.toLocaleTimeString()
}

// =============================
// Fetch Workouts on Mount
// =============================
onMounted(async () => {
  loading.value = true
  error.value = null
  try {
    workouts.value = await getWorkouts()
    console.log('Fetched workouts:', workouts.value) // Debug
  } catch (err: any) {
    error.value = err.response?.data?.message || err.message || 'Failed to load workouts'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <top-layout>
    <div class="workouts-page">
      <h1>My Workouts</h1>

      <!-- Loading -->
      <p v-if="loading">Loading workouts...</p>

      <!-- Error -->
      <p v-else-if="error" class="error">{{ error }}</p>

      <!-- Empty -->
      <p v-else-if="workouts.length === 0">No workouts found.</p>

      <!-- Workouts List -->
      <ul v-else>
        <li v-for="w in workouts" :key="w.id">
          <router-link :to="{ path: `/workouts/${w.id}` }">
            <strong>{{ w.name }}</strong> — {{ formatDate(w.date) }}
          </router-link>
        </li>
      </ul>
    </div>
  </top-layout>
</template>

<style scoped>
.workouts-page {
  margin: 0 auto;
}

.error {
  color: red;
  font-weight: bold;
}
</style>
