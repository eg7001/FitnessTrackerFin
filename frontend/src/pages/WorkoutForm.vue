<template>
  <TopLayout>
    <div class="form-container">
      <h1>Create Workout</h1>

      <form @submit.prevent="handleSubmit">
        <!-- Name -->
        <div class="form-group">
          <label>Workout Name</label>
          <input v-model="name" required />
        </div>

        <!-- Date -->
        <div class="form-group">
          <label>Date</label>
          <input v-model="date" type="datetime-local" />
        </div>

        <!-- Submit -->
        <button :disabled="loading">
          {{ loading ? 'Creating...' : 'Create Workout' }}
        </button>

        <!-- Error -->
        <p v-if="error" class="error">{{ error }}</p>
      </form>
    </div>
  </TopLayout>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import TopLayout from '@/components/TopLayout.vue'
import { createWorkout } from '@/services/workoutService'

const name = ref('')
const date = ref('')
const loading = ref(false)
const error = ref('')

const router = useRouter()

async function handleSubmit() {
  error.value = ''
  loading.value = true

  try {
    await createWorkout(name.value, date.value || undefined)

    // 🔥 redirect after success
    router.push('/workouts')
  } catch (err: any) {
    error.value = err.response?.data?.message || 'Failed to create workout'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.form-container {
  max-width: 400px;
}

.form-group {
  margin-bottom: 15px;
}

input {
  width: 100%;
  padding: 8px;
}

button {
  width: 100%;
  padding: 10px;
  background: #2563eb;
  color: white;
  border: none;
  border-radius: 6px;
}

.error {
  color: red;
  margin-top: 10px;
}
</style>
