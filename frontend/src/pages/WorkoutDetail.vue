<script lang="ts" setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import TopLayout from '@/components/TopLayout.vue'
import api from '@/services/api'

// ✅ USE YOUR TYPES
import type { Workout, ExerciseSet } from '@/types/workout'

// =============================
// State
// =============================
const workout = ref<Workout | null>(null)
const loading = ref(false)
const error = ref<string | null>(null)

const newExerciseName = ref('')
const newMuscleGroup = ref('')

// =============================
// Route
// =============================
const route = useRoute()
const workoutId = route.params.id as string

// =============================
// Fetch workout
// =============================
async function fetchWorkout() {
  try {
    loading.value = true
    const res = await api.get(`/workouts/${workoutId}`)
    workout.value = res.data
  } catch (err: any) {
    error.value = err.message || 'Failed to load'
  } finally {
    loading.value = false
  }
}

onMounted(fetchWorkout)

// =============================
// Add Exercise
// =============================
async function addExercise() {
  if (!newExerciseName.value) return

  await api.post(`/workouts/${workoutId}/exercises`, {
    exerciseName: newExerciseName.value,
    muscleGroup: newMuscleGroup.value,
  })

  newExerciseName.value = ''
  newMuscleGroup.value = ''

  await fetchWorkout()
}

// =============================
// Add Set
// =============================
async function addSet(exerciseId: number) {
  await api.post(`/workout-exercises/${exerciseId}/sets`, {
    reps: 10,
    weight: 50,
    isFailure: false,
  })

  await fetchWorkout()
}

// =============================
// Delete Set
// =============================
async function deleteSet(setId: number) {
  await api.delete(`/sets/${setId}`)
  await fetchWorkout()
}

// =============================
// Helpers
// =============================
const formatDate = (dateStr: string) => {
  const d = new Date(dateStr)
  return d.toLocaleDateString() + ' ' + d.toLocaleTimeString()
}
const formatSet = (s: ExerciseSet) => {
  return `Reps: ${s.reps}, Weight: ${s.weight}, Failure: ${s.isFailure ? 'Yes' : 'No'}`
}
</script>

<template>
  <TopLayout>
    <div class="workout-detail-page">
      <!-- Loading -->
      <p v-if="loading">Loading...</p>

      <!-- Error -->
      <p v-else-if="error">{{ error }}</p>

      <!-- ✅ ONLY render when workout EXISTS -->
      <div v-else-if="workout">
        <h1>{{ workout.name }}</h1>
        <p>{{ formatDate(workout.date) }}</p>

        <!-- ADD EXERCISE -->
        <div class="add-exercise">
          <input v-model="newExerciseName" placeholder="Exercise name" />
          <input v-model="newMuscleGroup" placeholder="Muscle group" />
          <button @click="addExercise">Add</button>
        </div>

        <!-- ✅ SAFE access -->
        <ul>
          <li v-for="we in workout.exercises" :key="we.id">
            <strong>{{ we.exerciseName }}</strong>
            <span v-if="we.muscleGroup"> ({{ we.muscleGroup }})</span>

            <button @click="addSet(we.id)">Add Set</button>

            <ul>
              <li v-for="s in we.sets" :key="s.id">
                {{ s.reps }} reps / {{ s.weight }}kg
                <button @click="deleteSet(s.id)">❌</button>
              </li>
            </ul>
          </li>
        </ul>
      </div>

      <!-- Fallback -->
      <p v-else>No workout found</p>
    </div>
  </TopLayout>
</template>

<style scoped>
.add-exercise {
  margin-bottom: 15px;
}

.add-exercise input {
  margin-right: 10px;
  padding: 5px;
}

button {
  margin-left: 5px;
}
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
