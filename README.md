# 👹 Gremlins

A WPF desktop app that subtly gaslights you while you work. Runs silently in the system tray and unleashes small, deniable chaos on your PC.

Nothing is destructive. Everything is plausibly your own fault.

**Docs:** [Contributing](CONTRIBUTING.md) · [Changelog](CHANGELOG.md) · [Security](SECURITY.md) · [License](LICENSE)

## Download

**[⬇️ Download Gremlins-Setup-1.0.0.exe](https://github.com/yourname/gremlins/releases/latest)** (after you publish a release)

Or grab the portable `Gremlins.exe` (no install needed) from the same page.

> Windows 10/11 only. A self-contained build does not require a separate .NET runtime.

## Gremlins

| Gremlin | What it does |
|---|---|
| 🖱️ **The Drifter** | Nudges your cursor a few pixels at random intervals |
| ⌨️ **The Typist** | Occasionally swaps a typed character for a visual lookalike (l→I, o→0, Cyrillic tricks) |
| 🧠 **The Amnesiac** | Silently clears your clipboard |
| 😔 **The Critic** | Plays a quiet disappointed sigh when you open social media or YouTube |
| 🦉 **The Philosopher** | Replaces your clipboard with an unsettling quote. You find out when you paste. |
| 👻 **The Lag Ghost** | Introduces fake input delay in bursts. Feels like your PC is crying. |
| 🪄 **The Rearranger** | Slowly shifts your active window's position. Nothing looks right. |

Each gremlin has three severity levels: **Mischievous → Annoying → Unhinged**.

## Stack

- **.NET 8 / WPF** — native Windows desktop (with `UseWindowsForms` for low-level hook message pumps)
- **CommunityToolkit.Mvvm** — MVVM (`ObservableObject`, `[RelayCommand]`, `[ObservableProperty]`)
- **Hardcodet.NotifyIcon.Wpf** — system tray
- **NAudio** — procedural sigh for The Critic
- **Newtonsoft.Json** — settings in `%APPDATA%\Gremlins\settings.json`
- **Win32 P/Invoke** — centralized in `Core/Win32.cs`

Implementations live in namespace **`Gremlins.Tricks`** (project folder `Gremlins/`).

## Getting Started

```bash
# Prerequisites: .NET 8 SDK, Windows

git clone https://github.com/yourname/gremlins.git
cd gremlins
dotnet run --project Gremlins.csproj
```

The app starts in the system tray only. Double-click the tray icon to open the dashboard.

## Building a release

Always publish the **project** file (not the `.sln`) when using `-o`:

```bash
dotnet publish Gremlins.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -p:PublishSingleFile=true `
  -o ./publish
```

Output: `publish/Gremlins.exe` (self-contained single file).

### Installer (Inno Setup)

1. Install [Inno Setup](https://jrsoftware.org/isinfo.php).
2. Publish to `./publish` as above.
3. Run: `iscc installer/gremlins.iss` from the repository root.
4. Output: `installer/output/Gremlins-Setup-1.0.0.exe`

### GitHub release

Push a version tag to trigger `.github/workflows/release.yml`:

```bash
git tag v1.0.0
git push origin v1.0.0
```

## Project Structure

```
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

## Adding Your Own Gremlin

1. Add a class under `Gremlins/` inheriting `BaseGremlin` in namespace `Gremlins.Tricks`.
2. Implement `Id`, `Name`, `Description`, `Emoji`, and `RunLoopAsync`.
3. Register it in `App.xaml.cs` (`services.AddSingleton<Tricks.YourGremlin>();`) and add it to `GremlinEngine.Initialise()`.

## Licence

Use at your own risk — and maybe don’t install this on someone else’s PC without consent.
