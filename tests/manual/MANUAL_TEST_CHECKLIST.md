# Gremlins — manual QA checklist

Use this when you need confidence beyond automated tests (hooks, tray, audio, and timing cannot be fully simulated in CI).

**Prep**

- [ ] `dotnet test Gremlins.sln -c Release` passes.
- [ ] Run `dotnet run --project Gremlins.csproj` from the repo root (or launch `Gremlins.exe` from `bin/`).
- [ ] Prefer a **non-production** Windows session or VM; tricks affect keyboard/mouse/clipboard globally.

**Legend**: Checkbox = pass/fail for each row.

---

## Gremlins (one at a time)

Disable **all** others while testing one gremlin. Use **Unhinged** only if you want fastest reproduction.

### The Drifter (cursor nudge)

- [ ] Enable only The Drifter; wait several minutes (or use Annoying/Unhinged for shorter intervals).
- [ ] Observe small mouse jumps while idle at desktop.

### The Typist (keyboard lookalikes)

- [ ] Enable only The Typist; focus Notepad or VS Code.
- [ ] Type lowercase `l`, `o`, `a`, etc. repeatedly until a substitution occurs (wrong Unicode homoglyph or lookalike).

### The Amnesiac (clipboard clear)

- [ ] Enable only The Amnesiac; copy some text; wait for interval.
- [ ] Paste — clipboard empty or cleared unexpectedly.

### The Philosopher (clipboard quote)

- [ ] Enable only The Philosopher; wait for interval.
- [ ] Paste — clipboard contains an absurd quote instead of what you copied.

### The Critic (audio + window title)

- [ ] Enable only The Critic; open a browser tab whose **window title** contains e.g. `youtube`, `reddit`, or `twitter`.
- [ ] Hear a short synthesized “sigh” (volume/OS mixer permitting).

### The Lag Ghost (mouse lag bursts)

- [ ] Enable only The Lag Ghost; wait until a lag burst starts (see Activity log line).
- [ ] Move mouse — movement feels delayed during burst.

### The Rearranger (window nudge)

- [ ] Enable only The Rearranger; focus any normal window; wait (long intervals on Mischievous).
- [ ] Window position shifts slightly.

---

## Execution gate & rules

Perform with **one** easy gremlin enabled (e.g. Typist on Mischievous) so you can tell “blocked vs allowed” quickly.

### Panic

- [ ] Tray → **Panic** — tricks stop (no substitutions / drifts) while toggles stay ON.
- [ ] Tray → **Resume** — behavior returns when rules allow.

### Quiet hours

- [ ] Rules tab: set quiet window to **include the current clock time**; enable quiet hours.
- [ ] Confirm gremlin actions stop; move window so current time is **outside** quiet range — actions resume.

### Schedule

- [ ] Enable schedule; set hour window to **exclude** current hour — blocked.
- [ ] Adjust window to **include** current hour — allowed.
- [ ] Toggle **Weekdays only** on a weekend (or opposite) — blocked when day disallowed.

### Foreground deny list

- [ ] Set deny list to an app you can focus (e.g. `notepad.exe`).
- [ ] With Notepad in foreground — blocked; switch to another app — allowed.

### Presentation / full-screen

- [ ] Start Windows **Presentation mode** or a full-screen game / exclusive app if available.
- [ ] With options enabled — blocked; exit presentation — allowed.

### Soft start

- [ ] Set **Soft start** to 2–3 minutes; restart Gremlins; confirm no tricks until window passes (watch Activity log / Typist).

### Idle intensity

- [ ] Enable idle intensity; leave PC idle past threshold; intervals should feel shorter for timer-based gremlins (optional / subjective).

---

## UI & app features

### Tabs & theme

- [ ] Switch themes (including a Prismatic theme); chrome and combo/list colors update.
- [ ] **Appearance** combo scrolls (many themes).

### Profiles

- [ ] **Quiet workplace** — all gremlins off.
- [ ] **Mild chaos** — all on, Mischievous.
- [ ] **Maximum chaos** — all on, Unhinged.

### Activity log

- [ ] Toggle logging; trigger Typist or Drifter; lines appear.
- [ ] **Clear log** empties list.

### UI sounds

- [ ] Enable UI sounds; toggle a gremlin — Asterisk sound (or system sound) plays.

### Start with Windows + delay

- [ ] Enable **Start with Windows**; set delay (e.g. 15 s); verify `HKCU\...\Run` entry contains PowerShell sleep (use `regedit` or script).
- [ ] Disable — Run entry removed (machine-specific).

### Portable mode

- [ ] **Enable portable** — `Gremlins.portable` appears next to exe; restart; data under `GremlinsData\` beside exe.
- [ ] **Use AppData again** — markers removed (confirm restart messaging).

### Recipes

- [ ] **Export recipe** JSON; **Import recipe** changes toggles/severities.

### Updates

- [ ] Enter a real `owner/repo` (optional); **Check GitHub** returns message; **Open releases** opens browser.

### Onboarding

- [ ] **Reset onboarding**; reopen dashboard — welcome dialog appears; complete checkbox flow.

### Tray

- [ ] Double-click tray — dashboard opens.
- [ ] **Exorcise** — process exits.

---

## Automated suite (reference)

```powershell
.\scripts\Run-Tests.ps1
```

---

## Notes

- **Hooks** require the process to keep running; closing the **window** does not stop gremlins (tray).
- If tests fail intermittently, run them on a clean user profile and disable other global hook tools.
