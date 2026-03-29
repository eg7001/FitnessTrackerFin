// src/types/workout.ts

// ✅ Define ExerciseSet as its own interface
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
  isBodyweight: boolean // moved from inside `exercise`
  sets: ExerciseSet[]
}

export interface Workout {
  id: string
  name: string
  date: string
  exercises: WorkoutExercise[]
}
