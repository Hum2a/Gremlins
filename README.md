# Gremlins

<div align="center">

**A WPF desktop app that subtly gaslights you while you work.**

Runs quietly in the system tray and applies small, deniable chaos to your PC.

*Nothing is destructive. Everything is plausibly your own fault.*

---

[Contributing](CONTRIBUTING.md) · [Changelog](CHANGELOG.md) · [Security](SECURITY.md) · [License](LICENSE)

</div>

---

## Contents

- [Download](#download)
- [The gremlins](#the-gremlins)
- [Stack](#stack)
- [Getting started](#getting-started)
- [Building a release](#building-a-release)
- [Project structure](#project-structure)
- [Adding your own gremlin](#adding-your-own-gremlin)
- [Licence](#licence)

---

## Download

| | |
| :-- | :-- |
| **App version:** | `1.0.0` |
| **Installer** | [Download Gremlins-Setup-1.0.0.exe](https://github.com/yourname/gremlins/releases/latest) — available after you publish a release |
| **Portable** | `Gremlins.exe` from the same release page — no install required |

> **Platform:** Windows 10 and 11. A self-contained build does not need a separate .NET runtime.

---

## The gremlins

Each gremlin has three severity levels: **Mischievous → Annoying → Unhinged**.

| Icon (Segoe MDL2) | Gremlin | What it does |
| :-- | :-- | :-- |
| TouchPointer | **The Drifter** | Nudges your cursor a few pixels at random intervals |
| KeyboardClassic | **The Typist** | Occasionally swaps a typed character for a visual lookalike (l→I, o→0, Cyrillic tricks) |
| Copy | **The Amnesiac** | Silently clears your clipboard |
| Volume | **The Critic** | Plays a quiet disappointed sigh when you open social media or YouTube |
| ReadingList | **The Philosopher** | Replaces your clipboard with an unsettling quote; you find out when you paste |
| Recent | **The Lag Ghost** | Introduces fake input delay in bursts — like your PC is crying |
| SwitchApps | **The Rearranger** | Slowly shifts your active window’s position so nothing quite lines up |

---

## Stack

| Layer | Technology |
| :-- | :-- |
| **UI** | .NET 8 / WPF (with `UseWindowsForms` for low-level hook message pumps) |
| **MVVM** | [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/) — `ObservableObject`, `[RelayCommand]`, `[ObservableProperty]` |
| **Tray** | [Hardcodet.NotifyIcon.Wpf](https://github.com/hardcodet/wpf-notifyicon) |
| **Audio** | [NAudio](https://github.com/naudio/NAudio) — procedural sigh for The Critic |
| **Settings** | [Newtonsoft.Json](https://www.newtonsoft.com/json) — `%APPDATA%\Gremlins\settings.json` |
| **Win32** | P/Invoke — centralized in `Core/Win32.cs` |

Implementations live in namespace **`Gremlins.Tricks`** (project folder `Gremlins/`).

---

## Getting started

**Prerequisites:** .NET 8 SDK on Windows.

```bash
git clone https://github.com/yourname/gremlins.git
cd gremlins
dotnet run --project Gremlins.csproj
```

The app starts in the **system tray only**. Double-click the tray icon to open the dashboard.

---

## Building a release

Always publish the **project** file (not the `.sln`) when using `-o`:

```powershell
dotnet publish Gremlins.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -p:PublishSingleFile=true `
  -o ./publish
```

**Output:** `publish/Gremlins.exe` (self-contained single file).

### Installer (Inno Setup)

| Step | Action |
| :--: | :-- |
| 1 | Install [Inno Setup](https://jrsoftware.org/isinfo.php). |
| 2 | Publish to `./publish` as above. |
| 3 | From the repository root: `iscc installer/gremlins.iss` |
| 4 | **Output:** `installer/output/Gremlins-Setup-1.0.0.exe` |

### GitHub release

Push a version tag to trigger `.github/workflows/release.yml`:

```bash
git tag v1.0.0
git push origin v1.0.0
```

---

## Project structure

```text
├── Core/
│   ├── IGremlin.cs
│   ├── BaseGremlin.cs
│   ├── GremlinEngine.cs
│   └── Win32.cs
├── Gremlins/              # namespace Gremlins.Tricks
│   ├── TheDrifter.cs
│   ├── TheTypist.cs
│   └── ...
├── Services/
│   ├── SettingsService.cs
│   └── RegistryStartupHelper.cs
├── UI/
│   ├── MainViewModel.cs
│   ├── GremlinCardViewModel.cs
│   ├── MainWindow.xaml
│   ├── Converters.cs
│   └── Styles.xaml
├── installer/
│   └── gremlins.iss
├── App.xaml
├── App.xaml.cs
└── IconGenerator.cs
```

---

## Adding your own gremlin

1. Add a class under `Gremlins/` inheriting `BaseGremlin` in namespace `Gremlins.Tricks`.
2. Implement `Id`, `Name`, `Description`, `IconGlyph` (Segoe MDL2 Assets code point), and `RunLoopAsync`.
3. Register it in `App.xaml.cs` (`services.AddSingleton<Tricks.YourGremlin>();`) and add it to `GremlinEngine.Initialise()`.

---

## Licence

Use at your own risk — and do not install this on someone else’s PC without their consent.
