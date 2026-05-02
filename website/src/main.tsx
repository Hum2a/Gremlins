import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { gremlinMarkUrl } from "./branding.ts";
import "./index.css";
import App from "./App.tsx";

function installFavicon(href: string) {
  const ensure = (rel: string, type?: string) => {
    let link = document.querySelector<HTMLLinkElement>(`link[rel="${rel}"]`);
    if (!link) {
      link = document.createElement("link");
      link.rel = rel;
      document.head.appendChild(link);
    }
    if (type) link.type = type;
    else link.removeAttribute("type");
    link.href = href;
  };
  ensure("icon", "image/svg+xml");
  ensure("apple-touch-icon");
}

installFavicon(gremlinMarkUrl);

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <App />
  </StrictMode>,
);
