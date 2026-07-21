import { describe, it, expect, beforeEach, vi } from 'vitest'

// stores/auth.ts creates its `token` ref once, the moment the module is
// first imported - it reads localStorage right then. That means every test
// below needs a *fresh* import (via vi.resetModules + dynamic import) after
// it has set up localStorage the way it wants, otherwise all tests would
// share one instance created on the very first import.
describe('useAuth store', () => {
  beforeEach(() => {
    localStorage.clear()
    vi.resetModules()
  })

  it('starts logged out when there is no token in localStorage', async () => {
    const { useAuth } = await import('@/stores/auth')
    const { isLoggedIn, token } = useAuth()

    expect(isLoggedIn.value).toBe(false)
    expect(token.value).toBeNull()
  })

  it('starts logged in when a token already exists in localStorage', async () => {
    localStorage.setItem('token', 'existing-token')

    const { useAuth } = await import('@/stores/auth')
    const { isLoggedIn, token } = useAuth()

    expect(isLoggedIn.value).toBe(true)
    expect(token.value).toBe('existing-token')
  })

  it('setToken updates reactive state and persists to localStorage', async () => {
    const { useAuth } = await import('@/stores/auth')
    const { setToken, isLoggedIn, token } = useAuth()

    setToken('new-token')

    expect(token.value).toBe('new-token')
    expect(isLoggedIn.value).toBe(true)
    expect(localStorage.getItem('token')).toBe('new-token')
  })

  it('logout clears reactive state and localStorage', async () => {
    localStorage.setItem('token', 'existing-token')

    const { useAuth } = await import('@/stores/auth')
    const { logout, isLoggedIn, token } = useAuth()

    logout()

    expect(token.value).toBeNull()
    expect(isLoggedIn.value).toBe(false)
    expect(localStorage.getItem('token')).toBeNull()
  })

  it('every call to useAuth() shares the same underlying state (singleton)', async () => {
    const { useAuth } = await import('@/stores/auth')
    const a = useAuth()
    const b = useAuth()

    a.setToken('shared-token')

    expect(b.token.value).toBe('shared-token')
    expect(b.isLoggedIn.value).toBe(true)
  })
})
