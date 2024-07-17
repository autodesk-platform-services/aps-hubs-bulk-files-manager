import {defineConfig} from 'vite'
import react from '@vitejs/plugin-react'
import mkcert from "vite-plugin-mkcert";

// https://vitejs.dev/config/

export default defineConfig({
    plugins: [react(), mkcert()]
    , server: {
        port: 3000,
        https: true,
        proxy: {
            '/api': {
                target: 'http://localhost:8083',
                changeOrigin: true,
                secure: false,
                rewrite: (path) => path.replace(/^\/api/, '/api')
            },
            '/hangfire': {
                target: 'http://localhost:8083',
                changeOrigin: true,
                secure: false,
                rewrite: (path) => path.replace(/^\/hangfire/, '/hangfire')
            },
            '/code': {
                target: 'http://localhost:8083',
                changeOrigin: true,
                secure: false,
                rewrite: (path) => path.replace(/^\/code/, '/code')
            }
        }
    }
})
