<script lang="ts" setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import type { Workout } from '@/types/workout'
import { getWorkoutById } from '@/services/workoutService'

const route = useRoute()
const workout = ref<Workout | null>(null)
const loading = ref(false)
const error = ref<string | null>(null)

const workoutId = route.params.id as string

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

function formatDate(dateStr: string) {
  const d = new Date(dateStr)
  return d.toLocaleDateString() + ' ' + d.toLocaleTimeString()
}
</script>

<template>
  <div class="workout-detail-page">
    <div v-if="!loading && workout">
      <h1>{{ workout.name }}</h1>
      <p>Date: {{ formatDate(workout.date) }}</p>

      <h2>Exercises</h2>
      <div v-if="workout.exercises.length === 0">No exercises in this workout.</div>
      <ul v-else>
        <li v-for="we in workout.exercises" :key="we.id">
          <strong>{{ we.exercise.name }}</strong>
          <ul>
            <li v-for="s in we.sets" :key="s.id">
              Reps: {{ s.reps }}, Weight: {{ s.weight }}, Failure: {{ s.isFailure ? 'Yes' : 'No' }}
            </li>
          </ul>
        </li>
      </ul>
    </div>
  </div>
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
