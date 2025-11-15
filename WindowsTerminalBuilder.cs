namespace RepoLauncher;

public class WindowsTerminalBuilder
{
    public string BuildCommand(RepoConfig repo)
    {
        var parts = new List<string>
        {
            "wt --window new --maximized",
            $"new-tab -d \"{repo.Path}\" wsl"
        };

        if (repo.Servers.Count > 0)
        {
            var firstServer = repo.Servers[0];
            var serverPath = string.IsNullOrEmpty(firstServer.SubPath)
                ? repo.Path
                : Path.Combine(repo.Path, firstServer.SubPath);
            parts.Add($"split-pane -H -d \"{serverPath}\" pwsh -NoExit -Command \"{firstServer.Command}\"");
        }

        parts.Add($"split-pane -V -d \"{repo.Path}\" pwsh -NoExit -Command gitui");
        parts.Add("focus-pane -t 0");
        parts.Add($"split-pane -V -d \"{repo.Path}\"");

        if (repo.Servers.Count > 1)
        {
            for (int i = 1; i < repo.Servers.Count; i++)
            {
                var server = repo.Servers[i];
                var serverPath = string.IsNullOrEmpty(server.SubPath)
                    ? repo.Path
                    : Path.Combine(repo.Path, server.SubPath);
                parts.Add($"split-pane -H -d \"{serverPath}\" pwsh -NoExit -Command \"{server.Command}\"");
            }
        }

        return string.Join(" ; ", parts);
    }
}
