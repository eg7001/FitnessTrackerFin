import { describe, it, expect, afterEach, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import TopLayout from '@/components/TopLayout.vue'
import { useAuth } from '@/stores/auth'

// TopLayout's logout button now calls the real authService.logout(), which
// hits the backend to revoke the refresh-token cookie before clearing local
// state. Faking the transport layer (api.ts) lets that real logic run
// end-to-end here without a real network call.
vi.mock('@/services/api', () => ({
  default: { post: vi.fn().mockResolvedValue({}) },
}))

const mockPush = vi.fn()
vi.mock('vue-router', async (importOriginal) => {
  const actual = await importOriginal<typeof import('vue-router')>()
  return {
    ...actual,
    useRouter: () => ({ push: mockPush }),
  }
})

describe('TopLayout', () => {
  afterEach(() => {
    // stores/auth.ts is a module-level singleton (see auth.spec.ts), so its
    // state leaks between tests in this file unless we explicitly reset it.
    // This is exactly the kind of "tests must be independent" gotcha that's
    // worth seeing once in a real codebase.
    useAuth().logout()
    mockPush.mockClear()
  })

  it('shows login/register links when logged out', () => {
    const wrapper = mount(TopLayout, {
      global: { stubs: { RouterLink: { template: '<a><slot /></a>' } } },
    })

    expect(wrapper.text()).toContain('Login')
    expect(wrapper.text()).toContain('Register')
    expect(wrapper.text()).not.toContain('Dashboard')
  })

  it('shows the nav links and a logout button when logged in', () => {
    useAuth().setToken('some-token')

    const wrapper = mount(TopLayout, {
      global: { stubs: { RouterLink: { template: '<a><slot /></a>' } } },
    })

    expect(wrapper.text()).toContain('Dashboard')
    expect(wrapper.text()).toContain('Workouts')
    expect(wrapper.text()).toContain('Logout')
  })

  it('clicking logout clears the auth token and redirects to login', async () => {
    useAuth().setToken('some-token')

    const wrapper = mount(TopLayout, {
      global: { stubs: { RouterLink: { template: '<a><slot /></a>' } } },
    })

    await wrapper.find('button').trigger('click')
    await vi.waitFor(() => {
      expect(useAuth().isLoggedIn.value).toBe(false)
    })

    expect(mockPush).toHaveBeenCalledWith('/login')
  })
})
