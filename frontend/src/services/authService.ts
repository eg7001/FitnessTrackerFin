import api from './api'

export async function login(email: string, password: string) {
  try {
    const response = await api.post('/auth/login', { email, password })

    const token = response.data.token
    localStorage.setItem('token', token)

    return response.data
  } catch (err: any) {
    console.error('Login failed:', err.response?.data || err)
    throw err
  }
}

export function logout() {
  localStorage.removeItem('token')
}

export function getToken() {
  return localStorage.getItem('token')
}
