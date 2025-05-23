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
	bool ReferencedBy(IValueModule module);
	bool DependsOn(IValueModule module);
	IReadOnlyDictionary<string, IReadOnlyModule> GetAllDependenciesReadOnly();
	IReadOnlyDictionary<string, IReadOnlyModule> GetAllReferencesReadOnly();
	IDictionary<string, HashSet<string>> GetRedundantDependencies(IDictionary<string, HashSet<string>>? unusedDependencies = null, HashSet<string>? visited = null);
	IDictionary<string, HashSet<string>> GetRedundantReferences(IDictionary<string, HashSet<string>>? unusedReferences = null, HashSet<string>? visited = null);
}