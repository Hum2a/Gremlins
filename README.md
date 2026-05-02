# 👹 Gremlins

A WPF desktop app that subtly gaslights you while you work. Runs silently in the system tray and unleashes small, deniable chaos on your PC.

Nothing is destructive. Everything is plausibly your own fault.

## Gremlins

| Gremlin | What it does |
|---|---|
| 🖱️ **The Drifter** | Nudges your cursor a few pixels at random intervals |
| ⌨️ **The Typist** | Occasionally swaps a typed character for a visual lookalike (l→I, o→0) |
| 🧠 **The Amnesiac** | Silently clears your clipboard |
| 😔 **The Critic** | Plays a quiet disappointed sigh when you open social media or YouTube |
| 🦉 **The Philosopher** | Replaces your clipboard with an unsettling quote. You find out when you paste. |
| 👻 **The Lag Ghost** | Introduces fake input delay for 30–90 seconds. Feels like your PC is crying. |
| 🪄 **The Rearranger** | Slowly shifts your active window's position. Nothing looks right. |

Each gremlin has three severity levels: **Mischievous → Annoying → Unhinged**.

## Stack

- **.NET 8 / WPF** — native Windows desktop
- **CommunityToolkit.Mvvm** — MVVM pattern, ObservableObject, RelayCommand
- **Hardcodet.NotifyIcon.Wpf** — system tray integration
- **NAudio** — audio generation for The Critic
- **P/Invoke / Win32** — mouse hooks, keyboard hooks, window manipulation

## Getting Started

```bash
# Prerequisites: .NET 8 SDK, Windows

git clone https://github.com/yourname/gremlins
cd Gremlins
dotnet run --project Gremlins/Gremlins.csproj
```

The app starts in the system tray. Double-click the 👹 icon to open the dashboard.

## Building a Release

```bash
dotnet publish Gremlins/Gremlins.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## Project Structure

```
Gremlins/
├── Core/
│   ├── IGremlin.cs          # Gremlin contract
│   ├── BaseGremlin.cs       # Shared loop/severity scaffolding
│   ├── GremlinEngine.cs     # Orchestrator
│   └── Win32.cs             # All P/Invoke declarations
├── Gremlins/
│   ├── TheDrifter.cs
│   ├── TheTypist.cs
│   ├── TheAmnesiac.cs
│   ├── TheCritic.cs
│   ├── ThePhilosopher.cs
│   ├── TheLagGhost.cs
│   └── TheRearranger.cs
├── Services/
│   └── SettingsService.cs   # JSON persistence
├── UI/
│   ├── MainViewModel.cs     # MVVM viewmodel
│   ├── MainWindow.xaml      # Dashboard
│   ├── MainWindow.xaml.cs
│   └── Styles.xaml          # Dark theme
├── App.xaml
├── App.xaml.cs              # DI, tray setup, lifecycle
└── IconGenerator.cs         # Runtime-generated tray icon
```

## Adding Your Own Gremlin

1. Create a new class in `Gremlins/` inheriting from `BaseGremlin`
2. Implement `Id`, `Name`, `Description`, `Emoji`, and `RunLoopAsync`
3. Register it in `App.xaml.cs` (`services.AddSingleton<YourGremlin>()`)
4. Add it to the list in `GremlinEngine.Initialise()`

That's it.
