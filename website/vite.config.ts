import react from "@vitejs/plugin-react";
import { defineConfig, loadEnv } from "vite";

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), "");
  return {
    plugins: [react()],
    /** Subpath deploys: set `VITE_BASE=/your-prefix/` in `.env` / CI (must include slashes). */
    base: env.VITE_BASE || "/",
  };
});
