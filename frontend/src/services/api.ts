import axios from 'axios'
import { getToken, setToken, refreshAccessToken, logout } from './authService'

// '/api' is same-origin: the Vite dev server proxies it to the backend in
// dev, nginx proxies it in the Docker build. That keeps the refresh-token
// cookie same-site and avoids CORS entirely for normal browser traffic.
const api = axios.create({
  baseURL: '/api',
  withCredentials: true,
})

api.interceptors.request.use((config) => {
  const token = getToken()
  if (token) config.headers['Authorization'] = `Bearer ${token}`
  return config
})

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config as any

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true

      try {
        const newToken = await refreshAccessToken()
        setToken(newToken)

        originalRequest.headers = originalRequest.headers || {}
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
