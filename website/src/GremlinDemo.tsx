import { useCallback, useEffect, useState } from "react";
import "./GremlinDemo.css";

/** Mirrors Gremlins.Tricks IDs / marketing strings from the desktop app. */
const GREMLINS = [
  {
    id: "the_drifter" as const,
    name: "The Drifter",
    emoji: "🖱️",
    description:
      "Nudges your cursor a few pixels when you're not looking. Completely deniable.",
  },
  {
    id: "the_typist" as const,
    name: "The Typist",
    emoji: "⌨️",
    description:
      "Occasionally swaps a character you typed for a lookalike. l→I, o→0, etc.",
  },
  {
    id: "the_amnesiac" as const,
    name: "The Amnesiac",
    emoji: "🧠",
    description:
      "Randomly clears your clipboard. You copied that, right? Are you sure?",
  },
  {
    id: "the_critic" as const,
    name: "The Critic",
    emoji: "😔",
    description:
      "Lets out a tiny sigh when you open social media or YouTube. It knows.",
  },
  {
    id: "the_philosopher" as const,
    name: "The Philosopher",
    emoji: "🦉",
    description:
      "Silently replaces your clipboard with a quote. You find out when you paste into a Teams message.",
  },
  {
    id: "the_lag_ghost" as const,
    name: "The Lag Ghost",
    emoji: "👻",
    description:
      "Introduces fake input delay in bursts. Feels like your PC is crying.",
  },
  {
    id: "the_rearranger" as const,
    name: "The Rearranger",
    emoji: "🪄",
    description:
      "Slowly shifts your active window's position over time. Nothing looks right but you can't explain why.",
  },
];

function DrifterDemo() {
  const [pos, setPos] = useState({ x: 52, y: 48 });
  useEffect(() => {
    const id = window.setInterval(() => {
      setPos((p) => ({
        x: Math.min(92, Math.max(8, p.x + (Math.random() - 0.5) * 14)),
        y: Math.min(88, Math.max(12, p.y + (Math.random() - 0.5) * 14)),
      }));
    }, 1600);
    return () => clearInterval(id);
  }, []);
  return (
    <div className="demo-drifter">
      <div className="demo-drifter-field" aria-hidden>
        <div
          className="demo-cursor"
          style={{ left: `${pos.x}%`, top: `${pos.y}%` }}
        />
      </div>
      <p className="demo-caption">Tiny random nudges—deniable at stand‑up.</p>
    </div>
  );
}

const LOOK: Record<string, string> = {
  l: "I",
  o: "0",
  O: "0",
  "1": "l",
};

function TypistDemo() {
  const phrase = "hello_world";
  const [shown, setShown] = useState("");
  useEffect(() => {
    let tick = -1;
    const id = window.setInterval(() => {
      tick += 1;
      const cycle = phrase.length + 14;
      const phase = tick % cycle;
      if (phase >= phrase.length) {
        setShown("");
        return;
      }
      const c = phrase[phase]!;
      const out = Math.random() < 0.2 && LOOK[c] ? LOOK[c]! : c;
      setShown((prev) => (phase === 0 ? out : prev + out));
    }, 200);
    return () => clearInterval(id);
  }, [phrase]);
  return (
    <div className="demo-typist">
      <div className="demo-typist-screen" aria-live="polite">
        <span className="demo-typist-text">{shown}</span>
        <span className="demo-typist-care">▍</span>
      </div>
      <p className="demo-caption">Sometimes your fingers—or the universe—slip.</p>
    </div>
  );
}

function AmnesiacDemo() {
  const [empty, setEmpty] = useState(false);
  useEffect(() => {
    const id = window.setInterval(() => setEmpty((e) => !e), 2800);
    return () => clearInterval(id);
  }, []);
  return (
    <div className="demo-amnesiac">
      <div className="demo-clip-card">
        <span className="demo-clip-label">Clipboard</span>
        <div className={`demo-clip-body ${empty ? "is-empty" : ""}`}>
          {empty ? (
            <span className="demo-clip-empty">Nothing here.</span>
          ) : (
            <code className="demo-clip-code">Quarterly_Report.pdf</code>
          )}
        </div>
      </div>
      <p className="demo-caption">Copy something important… then doubt reality.</p>
    </div>
  );
}

function CriticDemo() {
  return (
    <div className="demo-critic">
      <div className="demo-mini-window">
        <div className="demo-mini-titlebar">
          <span className="demo-mini-dot r" />
          <span className="demo-mini-dot y" />
          <span className="demo-mini-dot g" />
          <span className="demo-mini-title">youtube — Demo Browser</span>
        </div>
        <div className="demo-mini-content">
          <div className="demo-sigh-wave" aria-hidden>
            <span className="demo-sigh-label">sigh</span>
          </div>
          <p className="demo-mini-hint">Opens a “distraction” window… tiny sigh.</p>
        </div>
      </div>
      <p className="demo-caption">Judgment is silent but acoustically disappointed.</p>
    </div>
  );
}

const PHILOSOPHER_SAMPLES = [
  "The real cursor was the one we drifted along the way.",
  "It works on my machine. Ship my machine.",
  "ERROR 404: Motivation not found.",
  "Your rubber duck is judging you.",
];

function PhilosopherDemo() {
  const [q, setQ] = useState(0);
  useEffect(() => {
    const id = window.setInterval(
      () => setQ((i) => (i + 1) % PHILOSOPHER_SAMPLES.length),
      4200,
    );
    return () => clearInterval(id);
  }, []);
  return (
    <div className="demo-philosopher">
      <div className="demo-paste-box">
        <span className="demo-paste-label">Paste preview</span>
        <p className="demo-paste-quote">{PHILOSOPHER_SAMPLES[q]}</p>
      </div>
      <p className="demo-caption">Clipboard “philosophy” appears when you least expect it.</p>
    </div>
  );
}

function LagGhostDemo() {
  const [pending, setPending] = useState(false);
  const [flash, setFlash] = useState(false);
  const onClick = useCallback(() => {
    setPending(true);
    window.setTimeout(() => {
      setPending(false);
      setFlash(true);
      window.setTimeout(() => setFlash(false), 320);
    }, 520);
  }, []);
  return (
    <div className="demo-lag">
      <button
        type="button"
        className={`demo-lag-btn ${pending ? "is-pending" : ""} ${flash ? "is-flash" : ""}`}
        onClick={onClick}
      >
        Click me
      </button>
      <p className="demo-caption">Clicks arrive fashionably late—in bursts.</p>
    </div>
  );
}

function RearrangerDemo() {
  const [nudge, setNudge] = useState({ x: 0, y: 0 });
  useEffect(() => {
    const id = window.setInterval(() => {
      setNudge({
        x: (Math.random() - 0.5) * 18,
        y: (Math.random() - 0.5) * 12,
      });
    }, 2200);
    return () => clearInterval(id);
  }, []);
  return (
    <div className="demo-rearranger">
      <div
        className="demo-fake-window"
        style={{ transform: `translate(${nudge.x}px, ${nudge.y}px)` }}
      >
        <div className="demo-fake-title">Important.docx</div>
        <div className="demo-fake-body">Nothing is aligned. Everything is fine.</div>
      </div>
      <p className="demo-caption">Your focused window takes a slow scenic drift.</p>
    </div>
  );
}

function DemoStage({ id }: { id: (typeof GREMLINS)[number]["id"] }) {
  switch (id) {
    case "the_drifter":
      return <DrifterDemo />;
    case "the_typist":
      return <TypistDemo />;
    case "the_amnesiac":
      return <AmnesiacDemo />;
    case "the_critic":
      return <CriticDemo />;
    case "the_philosopher":
      return <PhilosopherDemo />;
    case "the_lag_ghost":
      return <LagGhostDemo />;
    case "the_rearranger":
      return <RearrangerDemo />;
    default:
      return null;
  }
}

export function GremlinDemo() {
  return (
    <section className="section demo-section" id="demo" aria-labelledby="demo-heading">
      <div className="section-head">
        <h2 id="demo-heading">Meet the gremlins</h2>
        <p>
          Safe, silly previews in your browser—nothing here touches your real
          mouse, keys, or clipboard. The Windows app runs with your rules,
          profiles, and panic switch.
        </p>
      </div>
      <ul className="demo-grid">
        {GREMLINS.map((g) => (
          <li key={g.id} className="demo-card">
            <div className="demo-card-head">
              <span className="demo-emoji" aria-hidden>
                {g.emoji}
              </span>
              <div>
                <h3>{g.name}</h3>
                <p className="demo-desc">{g.description}</p>
              </div>
            </div>
            <div className="demo-stage">
              <DemoStage id={g.id} />
            </div>
          </li>
        ))}
      </ul>
    </section>
  );
}
