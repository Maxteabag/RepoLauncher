namespace RepoLauncher;

public class RepoSelector
{
    private readonly AppSettings _settings;

    public RepoSelector(AppSettings settings)
    {
        _settings = settings;
    }

    public async Task<string?> SelectRepoAsync()
    {
        Console.WriteLine("Recent repositories:");
        Console.WriteLine();

        var recentRepos = _settings.RecentRepos.OrderByDescending(r => r.LastAccessed).ToList();
        for (int i = 0; i < recentRepos.Count && i < 10; i++)
        {
            Console.WriteLine($"{i + 1}. {recentRepos[i].Name}");
        }

        Console.WriteLine();
        Console.WriteLine("Enter number to select, or type repo name to search:");
        Console.Write("> ");

        var input = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(input)) return null;

        if (int.TryParse(input, out int index) && index > 0 && index <= recentRepos.Count)
        {
            return recentRepos[index - 1].Path;
        }

        var matches = await SearchReposAsync(input);
        if (matches.Count == 0)
        {
            Console.WriteLine("No repositories found.");
            return null;
        }

        if (matches.Count == 1)
        {
            Console.WriteLine($"Found: {matches[0]}");
            return matches[0];
        }

        Console.WriteLine();
        Console.WriteLine("Multiple matches found:");
        for (int i = 0; i < matches.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {Path.GetFileName(matches[i])}");
        }

        Console.Write("Select number: ");
        var selection = Console.ReadLine()?.Trim();
        if (int.TryParse(selection, out int matchIndex) && matchIndex > 0 && matchIndex <= matches.Count)
        {
            return matches[matchIndex - 1];
        }

        return null;
    }

    private async Task<List<string>> SearchReposAsync(string searchTerm)
    {
        if (!Directory.Exists(_settings.RootFolder))
        {
            return new List<string>();
        }

        return await Task.Run(() =>
        {
            return Directory.GetDirectories(_settings.RootFolder)
                .Where(d => Path.GetFileName(d).Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .OrderBy(d => Path.GetFileName(d))
                .ToList();
        });
    }
}
