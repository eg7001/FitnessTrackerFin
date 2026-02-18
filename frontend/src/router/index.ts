import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'

import Dashboard from '../pages/Dashboard.vue'
import Workouts from '../pages/Workouts.vue'
import WorkoutForm from '../pages/WorkoutForm.vue'
import Exercises from '../pages/Exercises.vue'
import Login from '../pages/Login.vue'

const routes: Array<RouteRecordRaw> = [
  { path: '/', component: Dashboard },
  { path: '/workouts', component: Workouts },
  { path: '/workouts/new', component: WorkoutForm },
  { path: '/exercises', component: Exercises },
  { path: '/login', component: Login },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
