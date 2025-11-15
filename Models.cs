namespace RepoLauncher;

public class AppSettings
{
    public string RootFolder { get; set; } = @"C:\GIT";
    public List<RepoConfig> RecentRepos { get; set; } = new();
}

public class RepoConfig
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public DateTime LastAccessed { get; set; }
    public List<ServerCommand> Servers { get; set; } = new();
}

public class ServerCommand
{
    public string SubPath { get; set; } = string.Empty;
    public string Command { get; set; } = string.Empty;
}
