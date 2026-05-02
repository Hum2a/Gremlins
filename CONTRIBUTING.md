# Contributing to Gremlins

Thanks for your interest. This guide covers how to build the project, match house style, and open a pull request.

<div align="center">

[README](README.md) · [Changelog](CHANGELOG.md) · [Security](SECURITY.md)

</div>

---

## Contents

- [Prerequisites](#prerequisites)
- [Build and run](#build-and-run)
- [Publish](#publish-self-contained-single-file)
- [Style](#style)
- [Pull requests](#pull-requests)
- [Code of conduct](#code-of-conduct)

---

## Prerequisites

| Requirement | Notes |
| :-- | :-- |
| **OS** | Windows 10 or 11 |
| **SDK** | [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) — see `global.json` for roll-forward behaviour |

---

## Build and run

```bash
dotnet build Gremlins.csproj -c Release
dotnet run --project Gremlins.csproj
```

---

## Publish (self-contained single file)

```bash
dotnet publish Gremlins.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish
```

---

## Style

- **EditorConfig** (`.editorconfig`) applies to C#, XAML, and project files — follow existing patterns in the repository.
- Keep pull requests **focused**: avoid drive-by refactors unless they are required for the change.

---

## Pull requests

1. **Describe** what changed and why (a short summary is enough).
2. **Confirm** you have not installed or deployed this app on anyone’s machine without their knowledge — see [SECURITY.md](SECURITY.md).

---

## Code of conduct

Be respectful. Pranks belong in the app, not in issues or reviews.
