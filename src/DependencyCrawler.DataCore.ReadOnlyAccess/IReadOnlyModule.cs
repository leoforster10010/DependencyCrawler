using DependencyCrawler.Data.Contracts;

namespace DependencyCrawler.DataCore.ReadOnlyAccess;

public interface IReadOnlyModule
{
	string Name { get; }
	IReadOnlyDictionary<string, IReadOnlyModule> DependenciesReadOnly { get; }
	IReadOnlyDictionary<string, IReadOnlyModule> ReferencesReadOnly { get; }
	int DependencyLayer { get; }
	int ReferenceLayer { get; }
	bool IsTopLevel { get; }
	bool IsSubLevel { get; }
	ModuleType Type { get; }
}