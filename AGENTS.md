# Repository Guidelines

## Project Structure & Module Organization
- Solution entry is `TcpExample.sln`; WPF source lives in `TcpExample/`.
- Application startup and resources: `TcpExample/App.xaml` and `App.xaml.cs`.
- Main window UI and code-behind: `TcpExample/MainWindow.xaml` and `MainWindow.xaml.cs`; keep UI wiring here and push logic into helper classes where possible.
- Generated resources/settings: `TcpExample/Properties/` (`Resources.resx`, `Settings.settings`, `AssemblyInfo.cs`).
- Build outputs land in `TcpExample/bin/Debug` or `TcpExample/bin/Release`; temporary artifacts sit in `TcpExample/obj/`.

## Build, Test, and Development Commands
- Build Debug: `msbuild TcpExample/TcpExample.csproj /p:Configuration=Debug`
- Build Release: `msbuild TcpExample/TcpExample.csproj /p:Configuration=Release`
- Clean: `msbuild TcpExample/TcpExample.csproj /t:Clean`
- Run locally: launch `TcpExample/bin/Debug/TcpExample.exe` after a Debug build, or start debugging via Visual Studio with the solution open.

## Coding Style & Naming Conventions
- Target framework is .NET Framework 4.8.1; keep code Windows-only and WPF-friendly.
- Use 4-space indentation; braces on new lines for types/methods (Visual Studio default).
- Naming: PascalCase for classes, methods, properties, events, and XAML control `x:Name`s (e.g., `ConnectButton`); camelCase for locals/parameters; `_camelCase` for private fields.
- Keep code-behind minimal; prefer separating UI from networking or data logic for testability.
- Update `App.config` if you add configuration (connection info, ports, timeouts) rather than hardcoding.

## Testing Guidelines
- No automated tests exist yet. When adding them, prefer a sibling `TcpExample.Tests` project using MSTest or NUnit; name files `MainWindowTests.cs` and keep one fixture per feature.
- Manual checks: build, run the app, and verify the main window loads without binding or startup exceptions; validate any TCP interactions you add against expected endpoints and error handling.

## Commit & Pull Request Guidelines
- Commits: concise, imperative subject lines that describe the behavior change (e.g., `Add basic TCP connect UI`); group related changes together.
- Pull requests: include a short summary, testing notes or repro steps, and screenshots/GIFs for UI updates. Link to any relevant work items or issues and call out risk areas (network changes, threading, UI responsiveness).

## Security & Configuration Tips
- Do not commit secrets, endpoints, or certificates; prefer app settings in `App.config` and document defaults.
- For networking changes, validate inputs and handle socket errors gracefully to avoid UI hangs; ensure long-running work stays off the UI thread.
