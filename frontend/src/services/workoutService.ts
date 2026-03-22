// src/services/workoutService.ts
import api from './api'
import type { Workout } from '@/types/workout'

export async function getWorkouts() {
  return (await api.get('/workouts')).data
}
export async function getWorkoutById(id: string): Promise<Workout> {
  const res = await api.get(`/workouts/${id}`)
  return res.data
}
