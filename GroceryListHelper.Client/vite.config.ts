import { defineConfig } from 'vite'
import { svelte } from '@sveltejs/vite-plugin-svelte'
import { fileURLToPath, URL } from 'node:url';
import { checker } from "vite-plugin-checker"
import tailwindcss from '@tailwindcss/vite';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [tailwindcss(), svelte(), checker({ typescript: true })],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  build: {
    outDir: '../GroceryListHelper.Server/wwwroot',
    emptyOutDir: true
  },
  server: {
    proxy: {
      '/api': {
        target: 'https://localhost:7021/',
        changeOrigin: true,
        secure: false
      },
      '/signin-oidc': {
        target: 'https://localhost:7021/',
        changeOrigin: true,
        secure: false
      }
    },
    port: 5173,
  }
})

