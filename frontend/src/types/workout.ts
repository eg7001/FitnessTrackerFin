// src/types/workout.ts

export interface ExerciseSet {
  id: number
  reps: number
  weight: number
  isFailure: boolean
}

export interface WorkoutExercise {
  id: number
  workoutId: string
  exerciseId: number
  exerciseName: string
  muscleGroup?: string
  isBodyweight: boolean
  sets: ExerciseSet[]
}

export interface Workout {
  id: string
  name: string
  date: string
  exercises: WorkoutExercise[]
}
