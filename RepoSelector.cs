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
        Console.WriteLine("Enter number to select, or start typing repo name:");
        Console.Write("> ");

        var input = await ReadWithAutocompleteAsync();
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
            Console.WriteLine($"Found: {Path.GetFileName(matches[0])}");
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

    private async Task<string> ReadWithAutocompleteAsync()
    {
        var input = string.Empty;
        var cursorLeft = Console.CursorLeft;
        var cursorTop = Console.CursorTop;

        while (true)
        {
            var keyInfo = Console.ReadKey(intercept: true);

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }

            if (keyInfo.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input = input[..^1];
                Console.SetCursorPosition(cursorLeft, cursorTop);
                Console.Write(new string(' ', Console.WindowWidth - cursorLeft));
                Console.SetCursorPosition(cursorLeft, cursorTop);
                Console.Write(input);
            }
            else if (keyInfo.Key == ConsoleKey.Tab)
            {
                var matches = await SearchReposAsync(input);
                if (matches.Count > 0)
                {
                    var suggestion = Path.GetFileName(matches[0]);
                    input = suggestion;
                    Console.SetCursorPosition(cursorLeft, cursorTop);
                    Console.Write(new string(' ', Console.WindowWidth - cursorLeft));
                    Console.SetCursorPosition(cursorLeft, cursorTop);
                    Console.Write(input);
                }
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                input += keyInfo.KeyChar;
                Console.SetCursorPosition(cursorLeft, cursorTop);
                Console.Write(input);

                var matches = await SearchReposAsync(input);
                if (matches.Count > 0)
                {
                    var suggestion = Path.GetFileName(matches[0]);
                    if (suggestion.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                    {
                        var completion = suggestion[input.Length..];
                        var savedForeground = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(completion);
                        Console.ForegroundColor = savedForeground;
                        Console.SetCursorPosition(cursorLeft + input.Length, cursorTop);
                    }
                }
            }
        }

        return input;
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
