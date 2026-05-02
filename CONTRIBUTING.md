# Contributing

Thanks for your interest in Gremlins.

## Prerequisites

- Windows 10 or 11
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (see `global.json` for roll-forward behaviour)

## Build & run

```bash
dotnet build Gremlins.csproj -c Release
dotnet run --project Gremlins.csproj
```

## Publish (self-contained single file)

```bash
dotnet publish Gremlins.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish
```

## Style

- **EditorConfig** (`.editorconfig`) applies to C#, XAML, and project files — match existing patterns in the repo.
- Avoid drive-by refactors in PRs; keep changes focused on the issue or feature.

## Pull requests

1. Describe what changed and why.
2. Confirm you have **not** installed or deployed this app on anyone’s machine without their knowledge (see `SECURITY.md`).

## Code of conduct

Be respectful. Pranks belong in the app, not in issues or reviews.
