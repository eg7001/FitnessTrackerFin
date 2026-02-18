<template>
  <TopLayout>
    <h1>Create New Workout</h1>
    <form @submit.prevent="submitWorkout" class="form">
      <label>
        Workout Name
        <input type="text" v-model="name" required />
      </label>

      <label>
        Date
        <input type="date" v-model="date" required />
      </label>

      <button type="submit" class="btn">Save</button>
    </form>
  </TopLayout>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import TopLayout from '../components/TopLayout.vue'

const name = ref('')
const date = ref('')

function submitWorkout() {
  console.log('Workout submitted:', { name: name.value, date: date.value })
  name.value = ''
  date.value = ''
  alert('Workout saved (mock)')
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
  background: #16a34a;
  color: white;
  border-radius: 6px;
  border: none;
  cursor: pointer;
}
</style>
