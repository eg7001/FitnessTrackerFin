<script lang="ts" setup>
// =============================
// Imports
// =============================
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/services/api'
import TopLayout from '@/components/TopLayout.vue'

// =============================
// Reactive Form State
// =============================
const router = useRouter()

const name = ref('')
const muscleGroup = ref('')
const isBodyweight = ref(false)
const loading = ref(false)
const error = ref<string | null>(null)

// =============================
// Submit Handler
// =============================
const submitForm = async () => {
  // Simple validation
  if (!name.value.trim()) {
    error.value = 'Exercise name is required'
    return
  }

  loading.value = true
  error.value = null

  try {
    const payload = {
      name: name.value.trim(),
      muscleGroup: muscleGroup.value.trim() || undefined,
      isBodyweight: isBodyweight.value,
    }

    const res = await api.post('/exercises', payload)
    console.log('Created exercise:', res.data)

    // Navigate back to exercises list
    router.push('/exercises')
  } catch (err: any) {
    error.value = err.response?.data?.message || err.message || 'Failed to create exercise'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <TopLayout>
    <div class="exercise-form-page">
      <h1>Create New Exercise</h1>

      <p v-if="error" class="error">{{ error }}</p>

      <form @submit.prevent="submitForm">
        <div class="form-group">
          <label for="name">Exercise Name</label>
          <input type="text" id="name" v-model="name" placeholder="e.g. Push-up" required />
        </div>

        <div class="form-group">
          <label for="muscleGroup">Muscle Group</label>
          <input type="text" id="muscleGroup" v-model="muscleGroup" placeholder="Optional" />
        </div>

        <div class="form-group">
          <label>
            <input type="checkbox" v-model="isBodyweight" />
            Bodyweight Exercise
          </label>
        </div>

        <button type="submit" :disabled="loading">
          {{ loading ? 'Creating...' : 'Create Exercise' }}
        </button>
      </form>
    </div>
  </TopLayout>
</template>

<style scoped>
.exercise-form-page {
  max-width: 600px;
  margin: 0 auto;
  padding: 1rem;
}

.form-group {
  margin-bottom: 1rem;
}

input[type='text'] {
  width: 100%;
  padding: 0.5rem;
  margin-top: 0.25rem;
}

button {
  padding: 0.5rem 1rem;
}

.error {
  color: red;
  font-weight: bold;
  margin-bottom: 1rem;
}
</style>
