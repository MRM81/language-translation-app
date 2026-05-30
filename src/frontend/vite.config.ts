import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      // Proxy all /api requests to the backend during local development.
      // This avoids CORS issues without requiring production CORS policy changes.
      '/api': {
        target: 'http://localhost:5074',
        changeOrigin: true,
      },
    },
  },
})
