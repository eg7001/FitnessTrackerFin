/// <reference types="vitest/config" />
import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    vueDevTools(),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    },
  },
  server: {
    proxy: {
      // Same-origin from the browser's point of view, so the backend's
      // httpOnly refresh-token cookie attaches to this (frontend) origin
      // instead of needing cross-site cookie rules.
      //
      // Deliberately targets the backend's plain-HTTP profile (not the
      // HTTPS one) so the backend sees a plain-HTTP request here, matching
      // the plain-HTTP page this dev server itself serves. The backend
      // marks the refresh cookie Secure based on the scheme it receives
      // (see AuthController) - if this proxied to HTTPS instead, the
      // cookie would come back Secure, and the browser would silently
      // refuse to store it because the page itself is on HTTP.
      '/api': {
        target: 'http://localhost:5187',
        changeOrigin: true,
      },
    },
  },
  test: {
    environment: 'jsdom',
    globals: true,
  },
})
