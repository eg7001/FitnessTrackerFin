import { describe, it, expect, beforeEach, vi } from 'vitest'

// The token ref is created once at module load (no longer seeded from
// localStorage), so each test re-imports the module fresh via
// vi.resetModules() to avoid state leaking between tests.
describe('useAuth store', () => {
  beforeEach(() => {
    vi.resetModules()
  })

  it('starts logged out with no token', async () => {
    const { useAuth } = await import('@/stores/auth')
    const { isLoggedIn, token } = useAuth()

    expect(isLoggedIn.value).toBe(false)
    expect(token.value).toBeNull()
  })

  it('setToken updates reactive state', async () => {
    const { useAuth } = await import('@/stores/auth')
    const { setToken, isLoggedIn, token } = useAuth()

    setToken('new-token')

    expect(token.value).toBe('new-token')
    expect(isLoggedIn.value).toBe(true)
  })

  it('logout clears reactive state', async () => {
    const { useAuth } = await import('@/stores/auth')
    const { setToken, logout, isLoggedIn, token } = useAuth()

    setToken('existing-token')
    logout()

    expect(token.value).toBeNull()
    expect(isLoggedIn.value).toBe(false)
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
