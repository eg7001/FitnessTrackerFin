<template>
  <TopLayout>
    <div class="register-container">
      <h1>Register</h1>

      <form @submit.prevent="handleRegister">
        <div class="form-group">
          <label>Email</label>
          <input v-model="email" type="email" required />
        </div>

        <div class="form-group">
          <label>Password</label>
          <input v-model="password" type="password" required />
        </div>

        <button type="submit" :disabled="loading">
          {{ loading ? 'Registering...' : 'Register' }}
        </button>

        <p v-if="error" class="error">{{ error }}</p>
      </form>
    </div>
  </TopLayout>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import TopLayout from '../components/TopLayout.vue'
import { register } from '../services/authService'

const email = ref('')
const password = ref('')
const loading = ref(false)
const error = ref('')

const router = useRouter()

async function handleRegister() {
  error.value = ''
  loading.value = true

  try {
    await register(email.value, password.value)

    // 👉 after register, go to login
    router.push('/login')
  } catch (err: any) {
    error.value = err.response?.data?.message || 'Registration failed'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.register-container {
  max-width: 400px;
}

.form-group {
  margin-bottom: 15px;
}

input {
  width: 100%;
  padding: 8px;
  margin-top: 5px;
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
  margin-top: 10px;
  color: red;
}
</style>
