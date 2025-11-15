using System.Text.Json;

namespace RepoLauncher;

public class ConfigManager
{
    public static readonly string ConfigPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "RepoLauncher",
        "settings.json"
    );

    public AppSettings LoadSettings()
    {
        if (!File.Exists(ConfigPath))
        {
            return new AppSettings();
        }

        var json = File.ReadAllText(ConfigPath);
        return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
    }

    public void SaveSettings(AppSettings settings)
    {
        var directory = Path.GetDirectoryName(ConfigPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(ConfigPath, json);
    }

    public void UpdateRecentRepo(AppSettings settings, RepoConfig repo)
    {
        repo.LastAccessed = DateTime.Now;

        var existing = settings.RecentRepos.FirstOrDefault(r => r.Path == repo.Path);
        if (existing != null)
        {
            settings.RecentRepos.Remove(existing);
        }

        settings.RecentRepos.Insert(0, repo);

        if (settings.RecentRepos.Count > 20)
        {
            settings.RecentRepos = settings.RecentRepos.Take(20).ToList();
        }

        SaveSettings(settings);
    }
}
