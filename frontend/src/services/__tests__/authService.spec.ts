import { describe, it, expect, vi, beforeEach } from 'vitest'

// authService now delegates all token state to the auth store instead of
// localStorage, so the store is faked here and asserted against directly.
const { mockToken, mockSetToken, mockClearToken } = vi.hoisted(() => {
  const mockToken = { value: null as string | null }
  return {
    mockToken,
    mockSetToken: vi.fn((t: string | null) => {
      mockToken.value = t
    }),
    mockClearToken: vi.fn(() => {
      mockToken.value = null
    }),
  }
})

vi.mock('@/stores/auth', () => ({
  useAuth: () => ({ token: mockToken, setToken: mockSetToken, logout: mockClearToken }),
}))

// api.ts (the shared axios instance) is faked so we only exercise
// authService's own logic.
vi.mock('@/services/api', () => ({
  default: {
    post: vi.fn(),
  },
}))

// refreshAccessToken deliberately uses a bare `axios.post`, not the shared
// `api` instance (see the comment in authService.ts) — mocked separately.
vi.mock('axios', () => ({
  default: {
    post: vi.fn(),
  },
}))

import api from '@/services/api'
import axios from 'axios'
import {
  login,
  register,
  logout,
  getToken,
  setToken,
  refreshAccessToken,
} from '@/services/authService'

describe('authService', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mockToken.value = null
  })

  describe('login', () => {
    it('stores the access token and returns it on success', async () => {
      vi.mocked(api.post).mockResolvedValue({ data: { accessToken: 'access-123' } })

      const result = await login('test@example.com', 'password')

      expect(api.post).toHaveBeenCalledWith('/auth/login', {
        email: 'test@example.com',
        password: 'password',
      })
      expect(result).toEqual({ accessToken: 'access-123' })
      expect(mockSetToken).toHaveBeenCalledWith('access-123')
    })

    it('throws and stores nothing when the access token is missing from the response', async () => {
      vi.mocked(api.post).mockResolvedValue({ data: {} })

      await expect(login('test@example.com', 'password')).rejects.toThrow(
        'Access token is missing',
      )
      expect(mockSetToken).not.toHaveBeenCalled()
    })

    it('propagates errors from the API', async () => {
      vi.mocked(api.post).mockRejectedValue(new Error('Invalid credentials'))

      await expect(login('test@example.com', 'wrong')).rejects.toThrow('Invalid credentials')
    })
  })

  describe('register', () => {
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

  describe('logout', () => {
    it('calls the logout endpoint and clears the store token', async () => {
      vi.mocked(api.post).mockResolvedValue({})

      await logout()

      expect(api.post).toHaveBeenCalledWith('/auth/logout')
      expect(mockClearToken).toHaveBeenCalled()
    })

    it('still clears the store token locally even if the request fails', async () => {
      vi.mocked(api.post).mockRejectedValue(new Error('network down'))

      await logout()

      expect(mockClearToken).toHaveBeenCalled()
    })
  })

  describe('token helpers', () => {
    it('getToken reads the current value from the store', () => {
      mockToken.value = 'abc'
      expect(getToken()).toBe('abc')
    })

    it('setToken writes through to the store', () => {
      setToken('xyz')
      expect(mockSetToken).toHaveBeenCalledWith('xyz')
    })
  })

  describe('refreshAccessToken', () => {
    it('exchanges the refresh cookie for a new access token and stores it', async () => {
      vi.mocked(axios.post).mockResolvedValue({ data: { accessToken: 'refreshed-token' } })

      const result = await refreshAccessToken()

      expect(axios.post).toHaveBeenCalledWith('/api/auth/refresh', null, {
        withCredentials: true,
      })
      expect(result).toBe('refreshed-token')
      expect(mockSetToken).toHaveBeenCalledWith('refreshed-token')
    })

    it('throws when the response has no access token', async () => {
      vi.mocked(axios.post).mockResolvedValue({ data: {} })

      await expect(refreshAccessToken()).rejects.toThrow('Access token is missing')
    })

    it('propagates errors, e.g. no valid refresh cookie', async () => {
      vi.mocked(axios.post).mockRejectedValue(new Error('401'))

      await expect(refreshAccessToken()).rejects.toThrow('401')
    })
  })
})
