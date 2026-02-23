// src/services/workoutService.ts
import api from './api'
import type { Workout } from '@/types/workout'

export async function getWorkouts(): Promise<Workout[]> {
  const response = await api.get('/workouts')
  console.log(response.data) // Debug the response
  return response.data
}
export async function getWorkoutById(id: string): Promise<Workout> {
  const res = await api.get(`/workouts/${id}`)
  return res.data
}
