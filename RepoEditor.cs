namespace RepoLauncher;

public class RepoEditor
{
    private readonly RepoConfigurator _configurator = new();

    public RepoConfig EditRepo(RepoConfig config)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine($"=== Editing: {config.Name} ===");
            Console.WriteLine();
            Console.WriteLine("Current servers:");

            if (config.Servers.Count == 0)
            {
                Console.WriteLine("  (none)");
            }
            else
            {
                for (int i = 0; i < config.Servers.Count; i++)
                {
                    var server = config.Servers[i];
                    var pathDisplay = string.IsNullOrEmpty(server.SubPath) ? "root" : server.SubPath;
                    Console.WriteLine($"  {i + 1}. {server.Command} (in {pathDisplay})");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  a. Add server");
            Console.WriteLine("  r. Remove server");
            Console.WriteLine("  e. Edit server");
            Console.WriteLine("  c. Clear all servers");
            Console.WriteLine("  d. Done editing");
            Console.Write("> ");

            var choice = Console.ReadLine()?.Trim().ToLower();

            switch (choice)
            {
                case "a":
                    AddServer(config);
                    break;

                case "r":
                    RemoveServer(config);
                    break;

                case "e":
                    EditServer(config);
                    break;

                case "c":
                    config.Servers.Clear();
                    Console.WriteLine("All servers cleared.");
                    break;

                case "d":
                    return config;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }

    private void AddServer(RepoConfig config)
    {
        Console.WriteLine();
        var subPath = _configurator.SelectSubPath(config.Path);

        Console.Write("Command to execute: ");
        var command = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(command))
        {
            Console.WriteLine("Command cannot be empty.");
            return;
        }

        config.Servers.Add(new ServerCommand
        {
            SubPath = subPath,
            Command = command
        });

        Console.WriteLine($"Added: {command} in {(string.IsNullOrEmpty(subPath) ? "root" : subPath)}");
    }

    private void RemoveServer(RepoConfig config)
    {
        if (config.Servers.Count == 0)
        {
            Console.WriteLine("No servers to remove.");
            return;
        }

        Console.Write("Enter server number to remove: ");
        var input = Console.ReadLine()?.Trim();

        if (int.TryParse(input, out int index) && index > 0 && index <= config.Servers.Count)
        {
            var removed = config.Servers[index - 1];
            config.Servers.RemoveAt(index - 1);
            Console.WriteLine($"Removed: {removed.Command}");
        }
        else
        {
            Console.WriteLine("Invalid server number.");
        }
    }

    private void EditServer(RepoConfig config)
    {
        if (config.Servers.Count == 0)
        {
            Console.WriteLine("No servers to edit.");
            return;
        }

        Console.Write("Enter server number to edit: ");
        var input = Console.ReadLine()?.Trim();

        if (int.TryParse(input, out int index) && index > 0 && index <= config.Servers.Count)
        {
            var server = config.Servers[index - 1];

            Console.WriteLine();
            Console.WriteLine($"Editing: {server.Command}");
            Console.WriteLine();
            Console.WriteLine("1. Change subpath");
            Console.WriteLine("2. Change command");
            Console.WriteLine("3. Change both");
            Console.Write("> ");

            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    server.SubPath = _configurator.SelectSubPath(config.Path);
                    break;

                case "2":
                    Console.Write("New command: ");
                    var newCommand = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrEmpty(newCommand))
                    {
                        server.Command = newCommand;
                    }
                    break;

                case "3":
                    server.SubPath = _configurator.SelectSubPath(config.Path);
                    Console.Write("New command: ");
                    var cmd = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrEmpty(cmd))
                    {
                        server.Command = cmd;
                    }
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }

            Console.WriteLine("Server updated.");
        }
        else
        {
            Console.WriteLine("Invalid server number.");
        }
    }
}
