import { ref, computed } from 'vue'

const token = ref<string | null>(localStorage.getItem('token'))

export function useAuth() {
  const isLoggedIn = computed(() => !!token.value)

  function setToken(newToken: string) {
    token.value = newToken
    localStorage.setItem('token', newToken)
  }

  function logout() {
    token.value = null
    localStorage.removeItem('token')
  }

  return {
    token,
    isLoggedIn,
    setToken,
    logout,
  }
}
