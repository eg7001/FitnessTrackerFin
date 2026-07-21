import { describe, it, expect, vi, beforeEach } from 'vitest'
import api from '@/services/api'
import { login, register, logout, getToken, getRefreshToken } from '@/services/authService'

// We only care about authService's own logic here, so the real api.ts
// (axios instance + interceptors) is replaced with a fake that just
// records calls. api.ts's own interceptor behavior is tested separately
// in api.spec.ts.
vi.mock('@/services/api', () => ({
  default: {
    post: vi.fn(),
  },
}))

describe('authService', () => {
  beforeEach(() => {
    localStorage.clear()
    vi.clearAllMocks()
  })

  describe('login', () => {
    it('stores both tokens and returns them on success', async () => {
      vi.mocked(api.post).mockResolvedValue({
        data: { accessToken: 'access-123', refreshToken: 'refresh-123' },
      })

      const result = await login('test@example.com', 'password')

      expect(api.post).toHaveBeenCalledWith('/auth/login', {
        email: 'test@example.com',
        password: 'password',
      })
      expect(result).toEqual({ accessToken: 'access-123', refreshToken: 'refresh-123' })
      expect(localStorage.getItem('token')).toBe('access-123')
      expect(localStorage.getItem('refreshToken')).toBe('refresh-123')
    })

    it('throws and stores nothing when the access token is missing from the response', async () => {
      vi.mocked(api.post).mockResolvedValue({ data: {} })

      await expect(login('test@example.com', 'password')).rejects.toThrow(
        'Access token is missing',
      )
      expect(localStorage.getItem('token')).toBeNull()
    })

    it('propagates errors from the API', async () => {
      vi.mocked(api.post).mockRejectedValue(new Error('Invalid credentials'))

      await expect(login('test@example.com', 'wrong')).rejects.toThrow('Invalid credentials')
    })
  })

  describe('register', () => {
    // Regression test: the backend's /auth/register endpoint does not return
    // an access token (it just creates the account), and Register.vue
    // redirects to /login afterward without needing one. register() must
    // not throw just because there's no accessToken in the response.
    it('resolves without requiring a token in the response', async () => {
      vi.mocked(api.post).mockResolvedValue({ data: {} })

      await expect(register('new@example.com', 'password')).resolves.toBeUndefined()
      expect(api.post).toHaveBeenCalledWith('/auth/register', {
        email: 'new@example.com',
        password: 'password',
      })
    })

    it('propagates errors from the API', async () => {
      vi.mocked(api.post).mockRejectedValue(new Error('Email already taken'))

      await expect(register('taken@example.com', 'password')).rejects.toThrow(
        'Email already taken',
      )
    })
  })

  describe('token helpers', () => {
    it('getToken reads the access token from localStorage', () => {
      localStorage.setItem('token', 'abc')
      expect(getToken()).toBe('abc')
    })

    it('getRefreshToken reads the refresh token from localStorage', () => {
      localStorage.setItem('refreshToken', 'xyz')
      expect(getRefreshToken()).toBe('xyz')
    })

    it('logout removes both tokens', () => {
      localStorage.setItem('token', 'abc')
      localStorage.setItem('refreshToken', 'xyz')

      logout()

      expect(localStorage.getItem('token')).toBeNull()
      expect(localStorage.getItem('refreshToken')).toBeNull()
    })
  })
})
