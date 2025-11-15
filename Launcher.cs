using System.Diagnostics;

namespace RepoLauncher;

public class Launcher
{
    private readonly ConfigManager _configManager = new();
    private readonly WindowsTerminalBuilder _terminalBuilder = new();
    private readonly RepoConfigurator _configurator = new();
    private readonly RepoEditor _editor = new();

    public async Task RunAsync(string[] args)
    {
        var cmdArgs = CommandLineArgs.Parse(args);

        if (cmdArgs.ShowHelp)
        {
            CommandLineArgs.DisplayHelp();
            return;
        }

        var settings = _configManager.LoadSettings();

        if (cmdArgs.ShowConfigPath)
        {
            Console.WriteLine($"Configuration file: {ConfigManager.ConfigPath}");
            return;
        }

        if (cmdArgs.OpenConfig)
        {
            OpenConfigFile();
            return;
        }

        if (!string.IsNullOrEmpty(cmdArgs.SetRootFolder))
        {
            SetRootFolder(settings, cmdArgs.SetRootFolder);
            return;
        }

        if (cmdArgs.ListRepos)
        {
            ListRepos(settings);
            return;
        }

        Console.WriteLine("=== Repo Launcher ===");
        Console.WriteLine();

        string? selectedPath;

        if (!string.IsNullOrEmpty(cmdArgs.RepoName))
        {
            selectedPath = FindRepoByName(settings, cmdArgs.RepoName);
            if (selectedPath == null)
            {
                Console.WriteLine($"Repository '{cmdArgs.RepoName}' not found.");
                return;
            }
            Console.WriteLine($"Launching: {cmdArgs.RepoName}");
        }
        else
        {
            var selector = new RepoSelector(settings);
            selectedPath = await selector.SelectRepoAsync();

            if (string.IsNullOrEmpty(selectedPath))
            {
                Console.WriteLine("No repository selected.");
                return;
            }
        }

        if (!Directory.Exists(selectedPath))
        {
            Console.WriteLine($"Directory not found: {selectedPath}");
            return;
        }

        var repoConfig = settings.RecentRepos.FirstOrDefault(r => r.Path == selectedPath);

        if (repoConfig == null)
        {
            repoConfig = _configurator.ConfigureRepo(selectedPath);
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine($"Found existing configuration for: {repoConfig.Name}");
            Console.WriteLine($"Servers configured: {repoConfig.Servers.Count}");
            Console.WriteLine();
            Console.WriteLine("e. Edit configuration");
            Console.WriteLine("d. Delete configuration");
            Console.WriteLine("l. Launch with current configuration");
            Console.Write("> ");

            var action = Console.ReadLine()?.Trim().ToLower();

            if (action == "e")
            {
                repoConfig = _editor.EditRepo(repoConfig);
            }
            else if (action == "d")
            {
                settings.RecentRepos.Remove(repoConfig);
                _configManager.SaveSettings(settings);
                Console.WriteLine("Configuration deleted. Please run again to reconfigure.");
                return;
            }
        }

        _configManager.UpdateRecentRepo(settings, repoConfig);

        Console.WriteLine();
        Console.WriteLine($"Launching: {repoConfig.Name}");
        Console.WriteLine();

        var wtCommand = _terminalBuilder.BuildCommand(repoConfig);

        var beforeWindows = GetWindowsTerminalHandles();

        var processStartInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c {wtCommand}",
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Process.Start(processStartInfo);

        await Task.Delay(2000);

        if (!cmdArgs.NoVSCode)
        {
            var vscodeStartInfo = new ProcessStartInfo
            {
                FileName = "code",
                Arguments = repoConfig.Path,
                UseShellExecute = true
            };

            Process.Start(vscodeStartInfo);
        }

        Console.WriteLine("Launch complete!");
    }

    private List<IntPtr> GetWindowsTerminalHandles()
    {
        try
        {
            return Process.GetProcessesByName("WindowsTerminal")
                .Where(p => p.MainWindowHandle != IntPtr.Zero)
                .Select(p => p.MainWindowHandle)
                .ToList();
        }
        catch
        {
            return new List<IntPtr>();
        }
    }

    private void OpenConfigFile()
    {
        if (!File.Exists(ConfigManager.ConfigPath))
        {
            Console.WriteLine($"Configuration file does not exist yet: {ConfigManager.ConfigPath}");
            Console.WriteLine("Run the launcher at least once to create it.");
            return;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = ConfigManager.ConfigPath,
            UseShellExecute = true
        };

        Process.Start(startInfo);
        Console.WriteLine($"Opened: {ConfigManager.ConfigPath}");
    }

    private void SetRootFolder(AppSettings settings, string rootFolder)
    {
        if (!Directory.Exists(rootFolder))
        {
            Console.WriteLine($"Directory does not exist: {rootFolder}");
            Console.Write("Create it? (y/n): ");
            var response = Console.ReadLine()?.Trim().ToLower();

            if (response == "y" || response == "yes")
            {
                Directory.CreateDirectory(rootFolder);
                Console.WriteLine($"Created: {rootFolder}");
            }
            else
            {
                Console.WriteLine("Root folder not changed.");
                return;
            }
        }

        settings.RootFolder = rootFolder;
        _configManager.SaveSettings(settings);
        Console.WriteLine($"Root folder set to: {rootFolder}");
    }

    private void ListRepos(AppSettings settings)
    {
        Console.WriteLine($"Root folder: {settings.RootFolder}");
        Console.WriteLine();
        Console.WriteLine("Configured repositories:");
        Console.WriteLine();

        if (settings.RecentRepos.Count == 0)
        {
            Console.WriteLine("  (none)");
            return;
        }

        var sorted = settings.RecentRepos.OrderByDescending(r => r.LastAccessed).ToList();
        for (int i = 0; i < sorted.Count; i++)
        {
            var repo = sorted[i];
            Console.WriteLine($"{i + 1}. {repo.Name}");
            Console.WriteLine($"   Path: {repo.Path}");
            Console.WriteLine($"   Servers: {repo.Servers.Count}");
            Console.WriteLine($"   Last accessed: {repo.LastAccessed:yyyy-MM-dd HH:mm}");
            Console.WriteLine();
        }
    }

    private string? FindRepoByName(AppSettings settings, string repoName)
    {
        var match = settings.RecentRepos.FirstOrDefault(r =>
            r.Name.Equals(repoName, StringComparison.OrdinalIgnoreCase));

        if (match != null)
        {
            return match.Path;
        }

        var repoPath = Path.Combine(settings.RootFolder, repoName);
        if (Directory.Exists(repoPath))
        {
            return repoPath;
        }

        return null;
    }
}
