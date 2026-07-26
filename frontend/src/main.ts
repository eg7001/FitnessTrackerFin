import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { refreshAccessToken } from './services/authService'

// The access token only lives in memory now, so a page reload starts with
// none. Try to silently trade the httpOnly refresh cookie (if any) for a
// fresh access token before the router's first navigation runs, so a
// logged-in user doesn't get bounced to /login on refresh. No cookie / an
// expired one just means the user is anonymous, which is fine.
async function bootstrap() {
  try {
    await refreshAccessToken()
  } catch {
    // no valid session — proceed logged out
  }

  createApp(App).use(router).mount('#app')
}

bootstrap()
