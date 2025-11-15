using System.Diagnostics;

namespace RepoLauncher;

public class Launcher
{
    private readonly ConfigManager _configManager = new();
    private readonly WindowsTerminalBuilder _terminalBuilder = new();
    private readonly RepoConfigurator _configurator = new();

    public async Task RunAsync(string[] args)
    {
        var settings = _configManager.LoadSettings();

        Console.WriteLine("=== Repo Launcher ===");
        Console.WriteLine();

        var selector = new RepoSelector(settings);
        var selectedPath = await selector.SelectRepoAsync();

        if (string.IsNullOrEmpty(selectedPath))
        {
            Console.WriteLine("No repository selected.");
            return;
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
                repoConfig = _configurator.ConfigureRepo(selectedPath);
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

        var vscodeStartInfo = new ProcessStartInfo
        {
            FileName = "code",
            Arguments = repoConfig.Path,
            UseShellExecute = true
        };

        Process.Start(vscodeStartInfo);

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
}
