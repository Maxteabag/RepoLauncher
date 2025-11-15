# Repo Launcher

Minimalistic .NET 10 console app for launching development workspaces with Windows Terminal and VSCode.

## Features

- Interactive repository selection with search
- Recent repositories tracking
- Configurable server commands per repo
- Automatic Windows Terminal layout with multiple panes
- VSCode integration

## Usage

```bash
dotnet run
```

## Configuration

Settings stored in: `%APPDATA%\RepoLauncher\settings.json`

Default root folder: `C:\GIT`

## Windows Terminal Layout

- Main pane: WSL shell
- Right pane: Server(s) running dev commands
- Bottom right: gitui for git operations
- Bottom left: Additional shell

## First Time Setup

When launching a new repo, you'll be prompted to configure:
- Optional server commands
- Subpaths for each server
- Commands to execute (e.g., `npm run dev`)
