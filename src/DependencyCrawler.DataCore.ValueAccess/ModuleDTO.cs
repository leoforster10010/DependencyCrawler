namespace DependencyCrawler.DataCore.ValueAccess;

public class ModuleDTO : IValueModule
{
	public required string Name { get; init; }
	public required IReadOnlyList<string> DependencyValues { get; init; }
	public required IReadOnlyList<string> ReferenceValues { get; init; }
}