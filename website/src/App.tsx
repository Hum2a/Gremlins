import "./App.css";

const supportEmail =
  import.meta.env.VITE_SUPPORT_EMAIL ?? "support@example.com";
const storeUrl = import.meta.env.VITE_MICROSOFT_STORE_URL;
const showDevHints = import.meta.env.DEV;

export default function App() {
  return (
    <div className="page">
      <div className="bg-grid" aria-hidden />
      <div className="bg-glow" aria-hidden />

      <header className="header">
        <div className="header-inner">
          <a className="brand" href="#top">
            <span className="brand-mark" aria-hidden>
              👹
            </span>
            <span className="brand-text">GREMLINS</span>
          </a>
          <nav className="nav" aria-label="Primary">
            <a href="#features">Features</a>
            <a href="#support">Support</a>
            <a href="#privacy">Privacy</a>
          </nav>
        </div>
      </header>

      <main id="top">
        <section className="hero">
          <p className="eyebrow">Windows · Tray app · Local-first</p>
          <h1 className="hero-title">
            Small chaos.
            <br />
            <span className="hero-title-accent">Big personality.</span>
          </h1>
          <p className="hero-lead">
            Gremlins is a desktop companion that subtly keeps you guessing—while
            you work. It runs quietly from the system tray, serves up tricks on
            your schedule, and includes a panic switch when you need peace.
          </p>
          <div className="hero-actions">
            {storeUrl ? (
              <a className="btn btn-primary" href={storeUrl}>
                Get it from the Microsoft Store
              </a>
            ) : (
              <span className="btn btn-primary btn-muted">
                Coming to the Microsoft Store
              </span>
            )}
            <a className="btn btn-ghost" href="#features">
              What it does
            </a>
          </div>
          {!storeUrl && showDevHints && (
            <p className="hero-note">
              Add your Store listing URL in{" "}
              <code className="inline-code">.env</code> as{" "}
              <code className="inline-code">VITE_MICROSOFT_STORE_URL</code>,
              then rebuild.
            </p>
          )}
        </section>

        <section className="section" id="features">
          <div className="section-head">
            <h2>Designed for focus—with a wink</h2>
            <p>
              Tune schedules, profiles, and themes. Each gremlin has its own
              mischief—discover them at your own risk.
            </p>
          </div>
          <ul className="feature-grid">
            <li className="card">
              <h3>Tray-first</h3>
              <p>
                Lives in the notification area. Open the dashboard when you
                want it; otherwise it stays out of the way.
              </p>
            </li>
            <li className="card">
              <h3>Roster of gremlins</h3>
              <p>
                Multiple personalities—movement, timing, audio, and more—each
                configurable so the chaos matches your day.
              </p>
            </li>
            <li className="card">
              <h3>Panic &amp; resume</h3>
              <p>
                Silence everything instantly, then bring tricks back when you
                are ready—without uninstalling or digging through menus.
              </p>
            </li>
            <li className="card">
              <h3>Themes &amp; polish</h3>
              <p>
                Dark-first UI that feels at home on Windows 11, with sounds and
                visuals you can tailor.
              </p>
            </li>
          </ul>
        </section>

        <section className="section section-alt" id="support">
          <div className="support-panel">
            <div>
              <h2>Support</h2>
              <p className="support-lead">
                This site is the official home for Gremlins on the web. For
                help, feedback, or weird bugs you cannot reproduce sober—reach
                out by email.
              </p>
            </div>
            <div className="support-actions">
              <a className="btn btn-primary" href={`mailto:${supportEmail}`}>
                Email support
              </a>
              {showDevHints && (
                <p className="support-email-hint">
                  Replace{" "}
                  <code className="inline-code">{supportEmail}</code> via{" "}
                  <code className="inline-code">VITE_SUPPORT_EMAIL</code> in{" "}
                  <code className="inline-code">.env</code>.
                </p>
              )}
            </div>
          </div>
        </section>

        <section className="section" id="privacy">
          <h2>Privacy</h2>
          <div className="legal-card">
            <p>
              Gremlins is built to run on your PC. It does not require an
              account to use core features, and it is not designed to upload
              your documents or keystrokes to our servers.
            </p>
            <p>
              Review this statement before you publish—tailor it to what your
              app actually does (telemetry, updates, crash reports, optional
              cloud features, etc.).
            </p>
            <p className="legal-muted">
              Last updated: replace this date when you ship. Link this section
              from Partner Center as your privacy policy URL once hosted.
            </p>
          </div>
        </section>
      </main>

      <footer className="footer">
        <p>
          © {new Date().getFullYear()} Gremlins. Made for Windows desktop.
        </p>
      </footer>
    </div>
  );
}
