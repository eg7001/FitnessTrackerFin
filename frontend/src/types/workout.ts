// src/types/workout.ts
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
  sets: {
    id: number
    reps: number
    weight: number
    isFailure: boolean
  }[]
}

export interface Workout {
  id: string
  name: string
  date: string
  exercises: WorkoutExercise[] // <-- match API property
}
