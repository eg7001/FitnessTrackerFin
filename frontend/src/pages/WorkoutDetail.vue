<script lang="ts" setup>
import { ref, onMounted, computed, onBeforeUnmount } from 'vue'
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

// to be deleted
const newExerciseName = ref('')
const newMuscleGroup = ref('')

const isOpen = ref(false)
const search = ref('')
const selectedExercise = ref<any | null>(null)

const filteredExercises = computed(() => {
  return exercisesList.value.filter((ex) =>
    ex.name.toLowerCase().includes(search.value.toLowerCase()),
  )
})

function selectExercise(ex: any) {
  selectedExercise.value = ex
  selectedExerciseId.value = ex.id
  search.value = ex.name
  isOpen.value = false
}

const groupedExercises = computed(() => {
  const filtered = exercisesList.value.filter((ex) =>
    ex.name.toLowerCase().includes(search.value.toLowerCase()),
  )

  const groups: Record<string, any[]> = {}

  for (const ex of filtered) {
    const group = ex.muscleGroup || 'Other'
    if (!groups[group]) {
      groups[group] = []
    }
    groups[group].push(ex)
  }

  return groups
})
// =============================
// Route
// =============================
const route = useRoute()
const workoutId = route.params.id as string

// EXERCISE
const exercisesList = ref<any[]>([])
const selectedExerciseId = ref<number | null>(null)

// =============================
// Fetch workout
// =============================
async function fetchWorkout() {
  try {
    loading.value = true
    error.value = null

    const res = await api.get(`/workouts/${workoutId}`)
    const data = res.data

    // Normalize exercises + sets safely
    const exercises = (data.exercises ?? data.workoutExercises ?? []).map((we: any) => ({
      id: we.id,
      workoutId: we.workoutId,
      exerciseId: we.exerciseId,
      exerciseName: we.exerciseName ?? we.exercise?.name ?? 'Unknown',
      muscleGroup: we.muscleGroup ?? we.exercise?.muscleGroup ?? '',
      isBodyweight: we.isBodyweight ?? false,
      sets: (we.sets ?? []).map((s: any) => ({
        id: s.id,
        reps: s.reps,
        weight: s.weight,
        isFailure: s.isFailure,
      })),
    }))

    workout.value = {
      id: data.id,
      name: data.name,
      date: data.date,
      exercises,
    }
  } catch (err: any) {
    error.value = err.message || 'Failed to load workout'
  } finally {
    loading.value = false
  }
}

async function fetchExercises() {
  try {
    const res = await api.get('/exercises')
    exercisesList.value = res.data
  } catch (err) {
    console.error('Failed to load exercises')
  }
}

function handleClickOutside(event: any) {
  if (!event.target.closest('.combo-box')) {
    isOpen.value = false
  }
}

onMounted(() => {
  fetchWorkout()
  ;(fetchExercises(), document.addEventListener('click', handleClickOutside))
})

onBeforeUnmount(() => {
  document.removeEventListener('click', handleClickOutside)
})
// =============================
// Add Exercise (OPTIMISTIC)
// =============================
async function addExercise() {
  if (!selectedExerciseId.value || !workout.value) return

  const tempId = Date.now()

  const selectedExercise = exercisesList.value.find((e) => e.id === selectedExerciseId.value)

  if (!selectedExercise) return

  const optimisticExercise = {
    id: tempId,
    workoutId: workout.value.id,
    exerciseId: selectedExercise.id,
    exerciseName: selectedExercise.name,
    muscleGroup: selectedExercise.muscleGroup,
    isBodyweight: selectedExercise.isBodyweight ?? false,
    sets: [],
  }

  // ✅ instant UI
  workout.value.exercises.unshift(optimisticExercise)

  try {
    const res = await api.post(`/workouts/${workoutId}/exercises`, {
      exerciseId: selectedExerciseId.value,
    })

    const saved = res.data

    const index = workout.value.exercises.findIndex((e) => e.id === tempId)
    if (index !== -1) {
      workout.value.exercises[index] = {
        ...optimisticExercise,
        id: saved.id, // important
      }
    }
  } catch (err) {
    // rollback
    workout.value.exercises = workout.value.exercises.filter((e) => e.id !== tempId)
  }
}

// =============================
// Add Set (OPTIMISTIC)
// =============================
async function addSet(exercise: any) {
  if (!workout.value) return

  console.log('Exercise passed:', exercise)

  if (!exercise.id) {
    console.error('❌ Missing workoutExerciseId', exercise)
    return
  }

  const tempId = Date.now()

  const lastSet = exercise.sets[exercise.sets.length - 1]

  const newSet = lastSet
    ? {
        reps: lastSet.reps,
        weight: lastSet.weight,
        isFailure: false,
      }
    : {
        reps: 10,
        weight: 20,
        isFailure: false,
      }

  const optimisticSet = {
    id: tempId,
    ...newSet,
  }

  exercise.sets.push(optimisticSet)

  try {
    const res = await api.post(`/workout-exercises/${exercise.id}/sets`, {
      reps: Number(newSet.reps),
      weight: Number(newSet.weight),
      isFailure: Boolean(newSet.isFailure),
    })
    const saved = res.data

    const index = exercise.sets.findIndex((s: ExerciseSet) => s.id === tempId)
    if (index !== -1) {
      exercise.sets[index] = saved
    }
  } catch (err) {
    console.error('❌ Failed to add set', err)
    exercise.sets = exercise.sets.filter((s: ExerciseSet) => s.id !== tempId)
  }
}

// =============================
// Delete Set (OPTIMISTIC)
// =============================
async function deleteSet(setId: number) {
  if (!workout.value) return

  let removedSet: any = null
  let parentExercise: any = null

  for (const ex of workout.value.exercises) {
    const index = ex.sets.findIndex((s) => s.id === setId)
    if (index !== -1) {
      removedSet = ex.sets[index]
      parentExercise = ex
      ex.sets.splice(index, 1)
      break
    }
  }

  try {
    await api.delete(`/sets/${setId}`)
  } catch (err) {
    if (parentExercise && removedSet) {
      parentExercise.sets.push(removedSet)
    }
  }
}

async function updateSet(set: any) {
  // store previous state (for rollback)
  const prev = { ...set }

  try {
    await api.put(`/sets/${set.id}`, {
      reps: set.reps,
      weight: set.weight,
      isFailure: set.isFailure,
    })
  } catch (err) {
    // rollback if failed
    set.reps = prev.reps
    set.weight = prev.weight
    set.isFailure = prev.isFailure
  }
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
        <div class="combo-box">
          <input v-model="search" @focus="isOpen = true" placeholder="Select exercise..." />

          <div v-if="isOpen" class="dropdown">
            <div
              v-for="ex in filteredExercises"
              :key="ex.id"
              class="option"
              @click="selectExercise(ex)"
            >
              {{ ex.name }} ({{ ex.muscleGroup }})
            </div>

            <div v-if="filteredExercises.length === 0" class="no-results">No results</div>
          </div>
        </div>

        <button @click="addExercise" :disabled="!selectedExerciseId">Add</button>
        <!-- ✅ SAFE access -->
        <ul>
          <li v-for="we in workout.exercises || []" :key="we.id">
            <strong>{{ we.exerciseName }}</strong>
            <span v-if="we.muscleGroup"> ({{ we.muscleGroup }})</span>

            <button @click="addSet(we)">Add Set</button>
            <ul>
              <li v-for="s in we.sets || []" :key="s.id" class="set-row">
                <input
                  type="number"
                  v-model.number="s.reps"
                  @change="updateSet(s)"
                  class="set-input"
                />

                <input
                  type="number"
                  v-model.number="s.weight"
                  @change="updateSet(s)"
                  class="set-input"
                />

                <label class="checkbox">
                  <input type="checkbox" v-model="s.isFailure" @change="updateSet(s)" />
                  Failure
                </label>

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
.combo-box {
  position: relative;
  width: 250px;
}

.combo-box input {
  width: 100%;
  padding: 8px;
  border: 1px solid #ccc;
  border-radius: 6px;
}

.dropdown {
  position: absolute;
  width: 100%;
  background: white;
  border: 1px solid #ccc;
  border-radius: 6px;
  max-height: 200px;
  overflow-y: auto;
  margin-top: 5px;
  z-index: 10;
}

.option {
  padding: 8px;
  cursor: pointer;
}

.option:hover {
  background: #f3f4f6;
}

.no-results {
  padding: 8px;
  color: #888;
}

.add-exercise {
  margin-bottom: 15px;
}

.add-exercise input {
  margin-right: 10px;
  padding: 5px;
}

.set-row {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 5px;
}

.set-input {
  width: 70px;
  padding: 5px;
}

.checkbox {
  display: flex;
  align-items: center;
  gap: 5px;
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
