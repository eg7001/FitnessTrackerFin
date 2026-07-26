import axios from 'axios'
import api from './api'
import { useAuth } from '@/stores/auth'

const { token, setToken: setStoreToken, logout: clearStoreToken } = useAuth()

export async function login(email: string, password: string) {
  try {
    const response = await api.post('/auth/login', { email, password })

    const accessToken = response.data.accessToken
    if (!accessToken) throw new Error('Access token is missing')

    setStoreToken(accessToken)

    return { accessToken }
  } catch (err: any) {
    console.error('Login failed:', err.response?.data || err)
    throw err
  }
}

export async function register(email: string, password: string) {
  try {
    // The register endpoint just creates the account and returns no token —
    // the caller (Register.vue) redirects to /login afterward, so there's
    // nothing here to store.
    await api.post('/auth/register', { email, password })
  } catch (err: any) {
    console.error('Registration failed:', err.response?.data || err)
    throw err
  }
}

export async function logout() {
  try {
    // Best-effort: revokes the refresh token and clears its cookie
    // server-side. Even if this fails (network down, already expired),
    // clearing the in-memory access token below still logs the user out
    // locally.
    await api.post('/auth/logout')
  } catch (err) {
    console.error('Logout request failed:', err)
  } finally {
    clearStoreToken()
  }
}

export function getToken() {
  return token.value
}

export function setToken(newToken: string | null) {
  setStoreToken(newToken)
}

// Exchanges the httpOnly refresh cookie for a new access token. Uses a bare
// axios call (not the shared `api` instance) so a failed refresh can't
// trigger api.ts's own 401 interceptor and recurse back into this function.
export async function refreshAccessToken(): Promise<string> {
  const res = await axios.post('/api/auth/refresh', null, { withCredentials: true })
  const accessToken = res.data.accessToken
  if (!accessToken) throw new Error('Access token is missing from refresh response')

  setStoreToken(accessToken)
  return accessToken
}
