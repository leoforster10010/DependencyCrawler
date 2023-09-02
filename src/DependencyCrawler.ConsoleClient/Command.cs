namespace DependencyCrawler.ConsoleClient;

public class Command
{
	public required int RequiredParameters { get; init; }
	public required string Description { get; init; }
	public required IList<string> CommandStrings { get; init; }
	public required Action<IList<string>> Action { get; init; }
}