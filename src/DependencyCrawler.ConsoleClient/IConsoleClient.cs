namespace DependencyCrawler.ConsoleClient;

public interface IConsoleClient
{
	void Run(CancellationToken cancellationToken);
}