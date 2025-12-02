namespace RepoLauncher;

public enum IdeType
{
    None,
    VSCode,
    VisualStudio,
    VisualStudioCode,
    Rider,
    IntelliJIdea,
    WebStorm,
    PyCharm,
    Custom
}

public class IdeConfig
{
    public IdeType Type { get; set; } = IdeType.VSCode;
    public string CustomExecutablePath { get; set; } = string.Empty;

    public string GetCommand()
    {
        return Type switch
        {
            IdeType.None => string.Empty,
            IdeType.VSCode => "code",
            IdeType.VisualStudioCode => "code",
            IdeType.VisualStudio => "devenv",
            IdeType.Rider => "rider",
            IdeType.IntelliJIdea => "idea",
            IdeType.WebStorm => "webstorm",
            IdeType.PyCharm => "pycharm",
            IdeType.Custom => CustomExecutablePath,
            _ => "code"
        };
    }

    public string GetDisplayName()
    {
        return Type switch
        {
            IdeType.None => "None (Terminal only)",
            IdeType.VSCode => "Visual Studio Code",
            IdeType.VisualStudioCode => "Visual Studio Code",
            IdeType.VisualStudio => "Visual Studio",
            IdeType.Rider => "JetBrains Rider",
            IdeType.IntelliJIdea => "IntelliJ IDEA",
            IdeType.WebStorm => "WebStorm",
            IdeType.PyCharm => "PyCharm",
            IdeType.Custom => $"Custom ({CustomExecutablePath})",
            _ => "Visual Studio Code"
        };
    }
}

public class AppSettings
{
    public string RootFolder { get; set; } = GetDefaultRootFolder();
    public List<RepoConfig> RecentRepos { get; set; } = new();
    public IdeConfig Ide { get; set; } = new();

    private static string GetDefaultRootFolder()
    {
        if (OperatingSystem.IsWindows())
        {
            return @"C:\GIT";
        }
        else
        {
            var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(homeDir, "GIT");
        }
    }
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
