// src/types/workout.ts

// ✅ Define ExerciseSet as its own interface
export interface ExerciseSet {
  id: number
  reps: number
  weight: number
  isFailure: boolean
}

// WorkoutExercise uses ExerciseSet[]
export interface WorkoutExercise {
  id: number
  workoutId: string
  exerciseId: number
  exerciseName: string // moved from inside `exercise`
  muscleGroup?: string // moved from inside `exercise`
  isBodyweight: boolean // moved from inside `exercise`
  sets: ExerciseSet[]
}
// Workout contains all exercises
export interface Workout {
  id: string
  name: string
  date: string
  exercises: WorkoutExercise[]
}
