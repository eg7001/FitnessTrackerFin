import { ref, computed } from 'vue'

// The access token lives in memory only - it is never written to
// localStorage or a JS-readable cookie, so an XSS bug can't read it out.
// It's short-lived and gets re-minted from the httpOnly refresh cookie
// (see authService.refreshAccessToken) whenever it's missing or expired.
const token = ref<string | null>(null)

export function useAuth() {
  const isLoggedIn = computed(() => !!token.value)

  function setToken(newToken: string | null) {
    token.value = newToken
  }

  function logout() {
    token.value = null
  }

  return {
    token,
    isLoggedIn,
    setToken,
    logout,
  }
}
