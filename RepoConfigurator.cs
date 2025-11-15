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

            Console.Write("SubPath (leave empty for root): ");
            var subPath = Console.ReadLine()?.Trim() ?? string.Empty;

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
}
