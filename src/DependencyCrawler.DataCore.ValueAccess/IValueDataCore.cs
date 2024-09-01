namespace DependencyCrawler.DataCore.ValueAccess;

public interface IValueDataCore
{
	Guid Id { get; }
	IReadOnlyList<string> ModuleValues { get; }
}