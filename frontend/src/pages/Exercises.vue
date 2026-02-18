<template>
  <TopLayout>
    <h1>Exercises</h1>

    <ul class="exercise-list">
      <li v-for="e in exercises" :key="e.id">{{ e.name }} - {{ e.muscle }}</li>
    </ul>
  </TopLayout>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import TopLayout from '../components/TopLayout.vue'

interface Exercise {
  id: number
  name: string
  muscle: string
}

const exercises = ref<Exercise[]>([])

onMounted(() => {
  exercises.value = [
    { id: 1, name: 'Bench Press', muscle: 'Chest' },
    { id: 2, name: 'Deadlift', muscle: 'Back' },
    { id: 3, name: 'Squat', muscle: 'Legs' },
  ]
})
</script>

<style scoped>
.exercise-list {
  list-style: none;
  padding: 0;
}

.exercise-list li {
  background: white;
  padding: 15px;
  margin-bottom: 10px;
  border-radius: 6px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}
</style>
