<script lang="ts" setup>
// =============================
// Imports
// =============================
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/services/api'
import TopLayout from '@/components/TopLayout.vue'

// =============================
// Types
// =============================
export interface Exercise {
  id: number
  name: string
  muscleGroup?: string
  isBodyweight: boolean
}

// =============================
// Reactive State
// =============================
const exercises = ref<Exercise[]>([])
const loading = ref(false)
const error = ref<string | null>(null)

const router = useRouter()

// =============================
// Fetch Exercises
// =============================
const getExercises = async () => {
  loading.value = true
  error.value = null

  try {
    const res = await api.get<Exercise[]>('/exercises')
    exercises.value = res.data
    console.log('Fetched exercises:', exercises.value)
  } catch (err: any) {
    error.value = err.response?.data?.message || err.message || 'Failed to load exercises'
  } finally {
    loading.value = false
  }
}

// Fetch on mount
onMounted(() => {
  getExercises()
})

// =============================
// Navigation
// =============================
const goToCreate = () => {
  router.push('/exercises/new')
}
</script>

<template>
  <TopLayout>
    <div class="exercises-page">
      <h1>Exercises</h1>

      <button @click="goToCreate">Create New Exercise</button>

      <!-- Loading -->
      <p v-if="loading">Loading exercises...</p>

      <!-- Error -->
      <p v-else-if="error" class="error">{{ error }}</p>

      <!-- Empty -->
      <p v-else-if="exercises.length === 0">No exercises found.</p>

      <!-- Exercises List -->
      <ul v-else>
        <li v-for="ex in exercises" :key="ex.id">
          <strong>{{ ex.name }}</strong>
          <em v-if="ex.muscleGroup"> — {{ ex.muscleGroup }}</em>
          <span v-if="ex.isBodyweight"> (Bodyweight)</span>
        </li>
      </ul>
    </div>
  </TopLayout>
</template>

<style scoped>
.exercises-page {
  max-width: 600px;
  margin: 0 auto;
  padding: 1rem;
}

button {
  margin-bottom: 1rem;
  padding: 0.5rem 1rem;
}

.error {
  color: red;
  font-weight: bold;
}
</style>
