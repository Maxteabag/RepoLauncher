namespace RepoLauncher;

public interface ITerminalBuilder
{
    string BuildCommand(RepoConfig repo);
    void Cleanup();
}
