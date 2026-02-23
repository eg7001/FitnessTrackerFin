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
  exercise: {
    id: number
    name: string
    muscleGroup?: string
    isBodyweight: boolean
  }
  sets: ExerciseSet[]
}

// Workout contains all exercises
export interface Workout {
  id: string
  name: string
  date: string
  exercises: WorkoutExercise[]
}
