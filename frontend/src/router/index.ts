import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'

import Home from '../pages/Home.vue'
import Dashboard from '../pages/Dashboard.vue'
import Workouts from '../pages/Workouts.vue'
import WorkoutForm from '../pages/WorkoutForm.vue'
import Exercises from '../pages/Exercises.vue'
import Login from '../pages/Login.vue'
import NotFound from '../pages/NotFound.vue'
import WorkoutDetail from '@/pages/WorkoutDetail.vue'
const routes: Array<RouteRecordRaw> = [
  { path: '/', component: Home },

  {
    path: '/dashboard',
    component: Dashboard,
    meta: { requiresAuth: true },
  },
  {
    path: '/workouts',
    component: Workouts,
    meta: { requiresAuth: true },
  },
  {
    path: '/workouts/new',
    component: WorkoutForm,
    meta: { requiresAuth: true },
  },
  {
    path: '/workouts/:id',
    component: WorkoutDetail,
    meta: { requiresAuth: true },
  },
  {
    path: '/exercises',
    component: Exercises,
    meta: { requiresAuth: true },
  },

  {
    path: '/login',
    component: Login,
    meta: { guestOnly: true },
  },

  { path: '/:pathMatch(.*)*', component: NotFound },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

/**
 * 🔐 Navigation Guard
 */
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token')

  if (to.meta.requiresAuth && !token) {
    next('/login')
  } else {
    next()
  }
})

export default router
