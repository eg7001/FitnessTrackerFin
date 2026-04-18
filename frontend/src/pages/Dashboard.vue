<template>
  <TopLayout>
    <h1>Dashboard</h1>

    <!-- STATS -->
    <div class="stats">
      <div class="card">
        <h3>Total Workouts</h3>
        <p>{{ stats.workouts }}</p>
      </div>

      <div class="card">
        <h3>This Week</h3>
        <p>{{ stats.weekly }}</p>
      </div>
    </div>

    <!-- RECENT -->
    <div class="recent-workouts">
      <h2>Recent Workouts</h2>

      <ul v-if="recent.length > 0">
        <li v-for="w in recent" :key="w.id" @click="goToWorkout(w.id)" class="clickable">
          <strong>{{ w.name }}</strong>
          <span>{{ new Date(w.date).toLocaleDateString() }}</span>
        </li>
      </ul>

      <p v-else>No workouts yet</p>
    </div>
  </TopLayout>
</template>
<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import TopLayout from '../components/TopLayout.vue'
import { getToken } from '@/services/authService'
import { getWorkouts } from '@/services/workoutService'

interface Workout {
  id: string
  name: string
  date: string
}

const workouts = ref<Workout[]>([])
const recent = ref<Workout[]>([])
const stats = ref({
  workouts: 0,
  weekly: 0,
})

const router = useRouter()

function getStartOfWeek() {
  const now = new Date()
  const day = now.getDay()
  const diff = now.getDate() - day + (day === 0 ? -6 : 1)
  return new Date(now.setDate(diff))
}

onMounted(async () => {
  const token = getToken()
  if (!token) {
    router.push('/login')
    return
  }

  try {
    const data: Workout[] = await getWorkouts()
    workouts.value = data

    // ✅ TOTAL WORKOUTS
    stats.value.workouts = data.length

    // ✅ WEEKLY WORKOUTS
    const startOfWeek = getStartOfWeek()

    stats.value.weekly = data.filter((w) => new Date(w.date) >= startOfWeek).length

    // ✅ RECENT WORKOUTS (last 5)
    recent.value = [...data]
      .sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
      .slice(0, 5)
  } catch (err) {
    console.error('Failed to load dashboard')
  }
})

function goToWorkout(id: string) {
  router.push(`/workouts/${id}`)
}
</script>
<style scoped>
.stats {
  display: flex;
  gap: 25px;
  margin: 20px 0;
}

.card {
  background: white;
  padding: 25px;
  border-radius: 8px;
  width: 200px;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
}

.card p {
  font-size: 28px;
  font-weight: bold;
}

.recent-workouts {
  margin-top: 40px;
}

.recent-workouts ul {
  list-style: none;
  padding: 0;
}

.recent-workouts li {
  background: white;
  padding: 15px;
  margin-bottom: 10px;
  border-radius: 6px;
  cursor: pointer;
  display: flex;
  justify-content: space-between;
}

.recent-workouts li:hover {
  background: #f3f4f6;
}

.clickable {
  transition: 0.2s;
}
</style>
