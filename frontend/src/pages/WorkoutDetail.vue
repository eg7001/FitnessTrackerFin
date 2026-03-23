<script lang="ts" setup>
// =============================
// Imports
// =============================
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import type { Workout, WorkoutExercise, ExerciseSet } from '@/types/workout'
import { getWorkoutById } from '@/services/workoutService'
import TopLayout from '@/components/TopLayout.vue'

// =============================
// Reactive State
// =============================
const workout = ref<Workout | null>(null)
const loading = ref(false)
const error = ref<string | null>(null)

// =============================
// Get route param safely
// =============================
const route = useRoute()
const workoutId = Array.isArray(route.params.id) ? route.params.id[0] : route.params.id

if (!workoutId) {
  throw new Error('No workout ID found in route params')
}

// =============================
// Helper Functions
// =============================

// Convert ISO date string to human-readable
const formatDate = (dateStr: string): string => {
  const d = new Date(dateStr)
  return d.toLocaleDateString() + ' ' + d.toLocaleTimeString()
}

// Format a single set
const formatSet = (s: ExerciseSet): string =>
  `Reps: ${s.reps}, Weight: ${s.weight}, Failure: ${s.isFailure ? 'Yes' : 'No'}`

// =============================
// Fetch Workout on Mount
// =============================
onMounted(async () => {
  loading.value = true
  error.value = null
  try {
    workout.value = await getWorkoutById(workoutId)
  } catch (err: any) {
    error.value = err.response?.data?.message || err.message || 'Failed to load workout'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <TopLayout>
    <div class="workout-detail-page">
      <!-- Loading -->
      <p v-if="loading">Loading workout...</p>

      <!-- Error -->
      <p v-else-if="error" class="error">{{ error }}</p>

      <!-- Workout Content -->
      <div v-else-if="workout">
        <h1>{{ workout.name }}</h1>
        <p>Date: {{ formatDate(workout.date) }}</p>

        <h2>Exercises</h2>
        <div v-if="workout.exercises.length === 0">No exercises in this workout.</div>

        <ul v-else>
          <li v-for="we in workout.exercises" :key="we.id">
            <strong>{{ we.exerciseName }}</strong>
            <em v-if="we.muscleGroup"> — {{ we.muscleGroup }}</em>

            <ul>
              <li v-for="s in we.sets" :key="s.id">{{ formatSet(s) }}</li>
            </ul>
          </li>
        </ul>
      </div>

      <!-- Fallback -->
      <p v-else>No workout data found.</p>
    </div>
  </TopLayout>
</template>

<style scoped>
.workout-detail-page {
  max-width: 700px;
  margin: 0 auto;
  padding: 1rem;
}
.error {
  color: red;
  font-weight: bold;
}
</style>
