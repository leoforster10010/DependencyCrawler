namespace DependencyCrawler.DataCore.ValueAccess;

public interface IValueModule
{
	string Name { get; }
	IReadOnlyList<string> DependencyValues { get; }
	IReadOnlyList<string> ReferenceValues { get; }
}