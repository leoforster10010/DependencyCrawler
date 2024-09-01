using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataCore.ReadOnlyAccess;

public interface IReadOnlyModule : IValueModule
{
	IReadOnlyDictionary<string, IReadOnlyModule> DependenciesReadOnly { get; }
	IReadOnlyDictionary<string, IReadOnlyModule> ReferencesReadOnly { get; }
	int DependencyLayer { get; }
	int ReferenceLayer { get; }
	bool IsTopLevel { get; }
	bool IsSubLevel { get; }
}