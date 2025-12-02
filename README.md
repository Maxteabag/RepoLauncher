# Repo Launcher

A fast cross-platform .NET global tool for launching development workspaces with your favorite terminal and IDE.

## Installation

```bash
dotnet tool install --global RepoLauncher
```

## Features

✨ **Quick Launch** - Type `rl` to instantly open your dev environment
🔍 **Smart Search** - Interactive autocomplete for finding repos
⚡ **Recent Repos** - Quick access to your most-used projects
🎛️ **Configurable** - Set up custom server commands per repo
📦 **Multi-Pane** - Automatic terminal layout with gitui
🎨 **Flexible** - Edit, delete, or reconfigure any repo setup
🖥️ **Cross-Platform** - Windows Terminal on Windows, Alacritty + Zellij on Linux

## Usage

### Interactive Mode
```bash
rl
```

### Launch Specific Repo
```bash
rl --repo MyProject
```

### Terminal Only (Skip VSCode)
```bash
rl --no-vscode --repo MyProject
```

### List All Configured Repos
```bash
rl --list
```

### Manage Settings
```bash
rl --set-root C:\Projects           # Change root folder
rl --config-path                    # Show config file location
rl --open-config                    # Edit config file
rl --help                           # Show all options
```

## Configuration

Settings stored in:
- **Windows**: `%APPDATA%\RepoLauncher\settings.json`
- **Linux**: `~/.config/RepoLauncher/settings.json`

Default root folder:
- **Windows**: `C:\GIT`
- **Linux**: `~/GIT`

### First Time Setup

When launching a new repo, you'll configure:
- **Server commands** - Optional dev servers (e.g., `npm run dev`)
- **Subpaths** - Interactive directory browser to select paths
- **Multiple servers** - Add as many as you need

### Editing Existing Repos

When you launch a configured repo, you can:
- **Launch** - Use existing configuration
- **Edit** - Add/remove/modify individual servers
- **Delete** - Remove configuration and start fresh

## Terminal Layout

### Windows (Windows Terminal)
- **Main pane**: WSL shell in repo root
- **Right pane**: First server (if configured)
- **Bottom right**: gitui for git operations
- **Bottom left**: Additional shell
- **Extra panes**: Additional servers (if configured)

### Linux (Alacritty + Zellij)
- **Left column**: Two shell panes in repo root
- **Right column**: First server (if configured) and gitui pane
- **Extra panes**: Additional servers (if configured)

## Requirements

### Common
- .NET 10 Runtime
- Your preferred IDE (optional): VSCode, Visual Studio, JetBrains IDEs, etc.
- gitui (optional, for git pane)

### Windows
- Windows Terminal

### Linux
- Alacritty
- Zellij

## Examples

```bash
# First time - configure MyProject with dev server
rl --repo MyProject

# Launch with terminal only
rl --no-vscode --repo MyAPI

# Browse and select from recent repos
rl

# Change your repos root folder
rl --set-root D:\Code

# See all configured repositories
rl --list
```

## Uninstall

```bash
dotnet tool uninstall --global RepoLauncher
```

## License

MIT
