namespace RepoLauncher;

public class ZellijTerminalBuilder : ITerminalBuilder
{
    private string? _layoutPath;

    public string BuildCommand(RepoConfig repo)
    {
        var layoutContent = BuildLayout(repo);
        _layoutPath = Path.Combine(Path.GetTempPath(), $"repol-{Guid.NewGuid()}.kdl");
        File.WriteAllText(_layoutPath, layoutContent);

        return $"alacritty -e zellij --layout \"{_layoutPath}\"";
    }

    private string BuildLayout(RepoConfig repo)
    {
        var layout = new System.Text.StringBuilder();

        layout.AppendLine("layout {");
        layout.AppendLine("    pane split_direction=\"vertical\" {");

        // Left side: Main shell + Additional shell below
        layout.AppendLine("        pane split_direction=\"horizontal\" {");
        layout.AppendLine($"            pane cwd=\"{repo.Path}\" {{");
        layout.AppendLine("                command \"claude\"");
        layout.AppendLine("                args \"--dangerously-skip-permissions\"");
        layout.AppendLine("            }");
        layout.AppendLine($"            pane cwd=\"{repo.Path}\"");
        layout.AppendLine("        }");

        // Right side panes
        layout.AppendLine("        pane split_direction=\"horizontal\" {");

        if (repo.Servers.Count > 0)
        {
            var firstServer = repo.Servers[0];
            var serverPath = string.IsNullOrEmpty(firstServer.SubPath)
                ? repo.Path
                : Path.Combine(repo.Path, firstServer.SubPath);

            layout.AppendLine($"            pane cwd=\"{serverPath}\" {{");
            layout.AppendLine($"                command \"{GetShellCommand()}\"");
            layout.AppendLine($"                args \"-c\" \"{EscapeCommand(firstServer.Command)}; exec $SHELL\"");
            layout.AppendLine("            }");
        }
        else
        {
            // Empty pane if no server
            layout.AppendLine($"            pane cwd=\"{repo.Path}\"");
        }

        // lazygit pane
        layout.AppendLine($"            pane cwd=\"{repo.Path}\" {{");
        layout.AppendLine("                command \"lazygit\"");
        layout.AppendLine("            }");

        layout.AppendLine("        }");

        // Additional server panes (if more than 1 server)
        if (repo.Servers.Count > 1)
        {
            for (int i = 1; i < repo.Servers.Count; i++)
            {
                var server = repo.Servers[i];
                var serverPath = string.IsNullOrEmpty(server.SubPath)
                    ? repo.Path
                    : Path.Combine(repo.Path, server.SubPath);

                layout.AppendLine($"        pane cwd=\"{serverPath}\" {{");
                layout.AppendLine($"            command \"{GetShellCommand()}\"");
                layout.AppendLine($"            args \"-c\" \"{EscapeCommand(server.Command)}; exec $SHELL\"");
                layout.AppendLine("            }");
            }
        }

        layout.AppendLine("    }");
        layout.AppendLine("}");

        return layout.ToString();
    }

    private string GetShellCommand()
    {
        var shell = Environment.GetEnvironmentVariable("SHELL");
        return string.IsNullOrEmpty(shell) ? "/bin/bash" : shell;
    }

    private string EscapeCommand(string command)
    {
        return command.Replace("\"", "\\\"").Replace("$", "\\$");
    }

    public void Cleanup()
    {
        if (_layoutPath != null && File.Exists(_layoutPath))
        {
            try
            {
                File.Delete(_layoutPath);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}
