import { describe, it, expect, afterEach } from 'vitest'
import { mount } from '@vue/test-utils'
import TopLayout from '@/components/TopLayout.vue'
import { useAuth } from '@/stores/auth'

describe('TopLayout', () => {
  afterEach(() => {
    // stores/auth.ts is a module-level singleton (see auth.spec.ts), so its
    // state leaks between tests in this file unless we explicitly reset it.
    // This is exactly the kind of "tests must be independent" gotcha that's
    // worth seeing once in a real codebase.
    useAuth().logout()
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

  it('clicking logout clears the auth token', async () => {
    useAuth().setToken('some-token')

    const wrapper = mount(TopLayout, {
      global: { stubs: { RouterLink: { template: '<a><slot /></a>' } } },
    })

    await wrapper.find('button').trigger('click')

    expect(useAuth().isLoggedIn.value).toBe(false)
    expect(localStorage.getItem('token')).toBeNull()
  })
})
