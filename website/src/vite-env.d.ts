/// <reference types="vite/client" />

declare module "*.svg?url" {
  const src: string;
  export default src;
}

interface ImportMetaEnv {
  readonly VITE_SUPPORT_EMAIL?: string;
  readonly VITE_MICROSOFT_STORE_URL?: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
