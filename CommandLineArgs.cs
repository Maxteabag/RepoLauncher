namespace RepoLauncher;

public class CommandLineArgs
{
    public bool ShowHelp { get; set; }
    public bool ListRepos { get; set; }
    public bool ShowConfigPath { get; set; }
    public bool OpenConfig { get; set; }
    public bool NoVSCode { get; set; }
    public string? RepoName { get; set; }
    public string? SetRootFolder { get; set; }

    public static CommandLineArgs Parse(string[] args)
    {
        var result = new CommandLineArgs();

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLower())
            {
                case "--help":
                case "-h":
                    result.ShowHelp = true;
                    break;

                case "--list":
                case "-l":
                    result.ListRepos = true;
                    break;

                case "--config-path":
                    result.ShowConfigPath = true;
                    break;

                case "--open-config":
                    result.OpenConfig = true;
                    break;

                case "--no-vscode":
                    result.NoVSCode = true;
                    break;

                case "--repo":
                case "-r":
                    if (i + 1 < args.Length)
                    {
                        result.RepoName = args[i + 1];
                        i++;
                    }
                    break;

                case "--set-root":
                    if (i + 1 < args.Length)
                    {
                        result.SetRootFolder = args[i + 1];
                        i++;
                    }
                    break;
            }
        }

        return result;
    }

    public static void DisplayHelp()
    {
        Console.WriteLine("=== Repo Launcher - Help ===");
        Console.WriteLine();
        Console.WriteLine("Usage: RepoLauncher [OPTIONS]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --help, -h              Show this help message");
        Console.WriteLine("  --list, -l              List all configured repositories");
        Console.WriteLine("  --repo, -r <name>       Launch specific repository by name");
        Console.WriteLine("  --no-vscode             Skip VSCode launch (terminal only)");
        Console.WriteLine("  --config-path           Show configuration file path");
        Console.WriteLine("  --open-config           Open configuration file in default editor");
        Console.WriteLine("  --set-root <path>       Set the root folder for repositories");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  rl                              # Interactive mode");
        Console.WriteLine("  rl --repo MyProject             # Launch MyProject directly");
        Console.WriteLine("  rl --list                       # Show all configured repos");
        Console.WriteLine("  rl --set-root C:\\Projects       # Change root folder");
        Console.WriteLine();
    }
}
