namespace RepoLauncher;

public class RepoConfigurator
{
    public RepoConfig ConfigureRepo(string repoPath)
    {
        var config = new RepoConfig
        {
            Name = Path.GetFileName(repoPath),
            Path = repoPath,
            LastAccessed = DateTime.Now
        };

        Console.WriteLine();
        Console.WriteLine($"Configuring: {config.Name}");
        Console.WriteLine();
        Console.WriteLine("Do you want to add server commands? (y/n)");
        Console.Write("> ");

        var response = Console.ReadLine()?.Trim().ToLower();
        if (response != "y" && response != "yes")
        {
            return config;
        }

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Enter server configuration:");

            var subPath = SelectSubPath(repoPath);

            Console.Write("Command to execute: ");
            var command = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(command))
            {
                Console.WriteLine("Command cannot be empty.");
                continue;
            }

            config.Servers.Add(new ServerCommand
            {
                SubPath = subPath,
                Command = command
            });

            Console.WriteLine();
            Console.WriteLine($"Added: {command} in {(string.IsNullOrEmpty(subPath) ? "root" : subPath)}");
            Console.WriteLine();
            Console.WriteLine("Add another server? (y/n)");
            Console.Write("> ");

            var another = Console.ReadLine()?.Trim().ToLower();
            if (another != "y" && another != "yes")
            {
                break;
            }
        }

        return config;
    }

    private string SelectSubPath(string repoPath)
    {
        var currentPath = repoPath;
        var relativePath = string.Empty;

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine($"Current path: {(string.IsNullOrEmpty(relativePath) ? "root" : relativePath)}");
            Console.WriteLine();

            var directories = Directory.GetDirectories(currentPath)
                .Select(d => Path.GetFileName(d))
                .Where(d => !string.IsNullOrEmpty(d))
                .OrderBy(d => d)
                .ToList();

            if (directories.Count > 0)
            {
                Console.WriteLine("Subdirectories:");
                for (int i = 0; i < directories.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {directories[i]}");
                }
                Console.WriteLine();
            }

            Console.WriteLine("0. Use current path");
            if (!string.IsNullOrEmpty(relativePath))
            {
                Console.WriteLine("b. Go back");
            }
            Console.WriteLine();
            Console.Write("Select option: ");

            var input = Console.ReadLine()?.Trim().ToLower();

            if (input == "0")
            {
                return relativePath;
            }

            if (input == "b" && !string.IsNullOrEmpty(relativePath))
            {
                var parent = Path.GetDirectoryName(relativePath);
                relativePath = parent ?? string.Empty;
                currentPath = string.IsNullOrEmpty(relativePath)
                    ? repoPath
                    : Path.Combine(repoPath, relativePath);
                continue;
            }

            if (int.TryParse(input, out int index) && index > 0 && index <= directories.Count)
            {
                var selectedDir = directories[index - 1];
                relativePath = string.IsNullOrEmpty(relativePath)
                    ? selectedDir
                    : Path.Combine(relativePath, selectedDir);
                currentPath = Path.Combine(repoPath, relativePath);
                continue;
            }

            Console.WriteLine("Invalid selection.");
        }
    }
}
