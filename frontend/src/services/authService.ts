import api from './api'

// =======================
// LOGIN
// =======================
export async function login(email: string, password: string) {
  try {
    const response = await api.post('/auth/login', { email, password })

    const token = response.data.accessToken
    const refreshToken = response.data.refreshToken

    if (!token) throw new Error('Access token is missing')

    localStorage.setItem('token', token)
    localStorage.setItem('refreshToken', refreshToken)

    return { accessToken: token, refreshToken }
  } catch (err: any) {
    console.error('Login failed:', err.response?.data || err)
    throw err
  }
}

export async function register(email: string, password: string) {
  try {
    const response = await api.post('/auth/register', { email, password })

    const token = response.data.accessToken
    const refreshToken = response.data.refreshToken

    if (!token) throw new Error('Access token is missing after registration')

    localStorage.setItem('token', token)
    localStorage.setItem('refreshToken', refreshToken)

    return { accessToken: token, refreshToken }
  } catch (err: any) {
    console.error('Registration failed:', err.response?.data || err)
    throw err
  }
}

export function logout() {
  localStorage.removeItem('token')
  localStorage.removeItem('refreshToken')
}

export function getToken() {
  return localStorage.getItem('token')
}

export function getRefreshToken() {
  return localStorage.getItem('refreshToken')
}
