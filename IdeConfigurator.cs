namespace RepoLauncher;

public class IdeConfigurator
{
    public IdeConfig ConfigureIde(IdeConfig? currentConfig = null)
    {
        var config = currentConfig ?? new IdeConfig();

        Console.WriteLine();
        Console.WriteLine("=== IDE Configuration ===");
        Console.WriteLine();
        Console.WriteLine("Select your preferred IDE:");
        Console.WriteLine();
        Console.WriteLine("1. Visual Studio Code (command: code)");
        Console.WriteLine("2. Visual Studio (command: devenv)");
        Console.WriteLine("3. JetBrains Rider (command: rider)");
        Console.WriteLine("4. IntelliJ IDEA (command: idea)");
        Console.WriteLine("5. WebStorm (command: webstorm)");
        Console.WriteLine("6. PyCharm (command: pycharm)");
        Console.WriteLine("7. Custom (specify your own executable)");
        Console.WriteLine("8. None (terminal only)");
        Console.WriteLine();
        Console.Write("> ");

        var choice = Console.ReadLine()?.Trim();

        switch (choice)
        {
            case "1":
                config.Type = IdeType.VSCode;
                break;

            case "2":
                config.Type = IdeType.VisualStudio;
                break;

            case "3":
                config.Type = IdeType.Rider;
                break;

            case "4":
                config.Type = IdeType.IntelliJIdea;
                break;

            case "5":
                config.Type = IdeType.WebStorm;
                break;

            case "6":
                config.Type = IdeType.PyCharm;
                break;

            case "7":
                config.Type = IdeType.Custom;
                ConfigureCustomPath(config);
                break;

            case "8":
                config.Type = IdeType.None;
                break;

            default:
                Console.WriteLine("Invalid selection, using Visual Studio Code as default.");
                config.Type = IdeType.VSCode;
                break;
        }

        if (config.Type != IdeType.None && config.Type != IdeType.Custom)
        {
            Console.WriteLine();
            Console.WriteLine($"Selected: {config.GetDisplayName()}");
            Console.WriteLine($"Command: {config.GetCommand()}");
            Console.WriteLine();
            Console.WriteLine("Would you like to customize the executable path? (y/n)");
            Console.Write("> ");

            var customize = Console.ReadLine()?.Trim().ToLower();
            if (customize == "y" || customize == "yes")
            {
                var originalType = config.Type;
                config.Type = IdeType.Custom;
                ConfigureCustomPath(config);

                if (string.IsNullOrEmpty(config.CustomExecutablePath))
                {
                    config.Type = originalType;
                }
            }
        }

        Console.WriteLine();
        Console.WriteLine($"IDE configured: {config.GetDisplayName()}");

        return config;
    }

    private void ConfigureCustomPath(IdeConfig config)
    {
        Console.WriteLine();
        Console.WriteLine("Enter the full path to your IDE executable:");
        Console.WriteLine("(e.g., C:\\Program Files\\MyIDE\\ide.exe)");
        Console.Write("> ");

        var path = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(path))
        {
            Console.WriteLine("No path provided.");
            return;
        }

        config.CustomExecutablePath = path;
        Console.WriteLine($"Custom path set: {path}");
    }

    public void DisplayCurrentIde(IdeConfig config)
    {
        Console.WriteLine();
        Console.WriteLine($"Current IDE: {config.GetDisplayName()}");

        if (config.Type != IdeType.None)
        {
            Console.WriteLine($"Command: {config.GetCommand()}");
        }
    }
}
