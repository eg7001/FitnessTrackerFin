import axios from 'axios'
import { getToken, getRefreshToken, logout } from './authService'

const api = axios.create({
  baseURL: 'https://localhost:7008/api',
})

api.interceptors.request.use((config) => {
  const token = getToken()
  if (token) config.headers['Authorization'] = `Bearer ${token}`
  return config
})

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true

      try {
        const refreshToken = getRefreshToken()
        if (!refreshToken) throw new Error('No refresh token available')

        const res = await axios.post('https://localhost:7008/api/auth/refresh', { refreshToken })

        const newToken = res.data.accessToken
        localStorage.setItem('token', newToken)

        originalRequest.headers['Authorization'] = `Bearer ${newToken}`
        return api(originalRequest)
      } catch (err) {
        logout()
        window.location.href = '/login'
        return Promise.reject(err)
      }
    }

    return Promise.reject(error)
  },
)

export default api
