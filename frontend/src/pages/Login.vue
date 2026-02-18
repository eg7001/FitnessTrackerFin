<template>
  <TopLayout>
    <h1>Login</h1>
    <form @submit.prevent="login" class="form">
      <label>
        Email
        <input type="email" v-model="email" required />
      </label>
      <label>
        Password
        <input type="password" v-model="password" required />
      </label>
      <button type="submit" class="btn">Login</button>
    </form>
  </TopLayout>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import TopLayout from '../components/TopLayout.vue'

const email = ref('')
const password = ref('')

function login() {
  console.log('Login attempt:', { email: email.value, password: password.value })
  alert('Login (mock) successful')
  email.value = ''
  password.value = ''
}
</script>

<style scoped>
.form {
  display: flex;
  flex-direction: column;
  max-width: 400px;
  gap: 15px;
}

.form label {
  display: flex;
  flex-direction: column;
  font-weight: bold;
}

.form input {
  padding: 8px 10px;
  margin-top: 5px;
  border-radius: 5px;
  border: 1px solid #ccc;
}

.btn {
  padding: 10px 15px;
  background: #2563eb;
  color: white;
  border-radius: 6px;
  border: none;
  cursor: pointer;
}
</style>
