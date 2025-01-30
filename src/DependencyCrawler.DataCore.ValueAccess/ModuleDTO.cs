namespace DependencyCrawler.DataCore.ValueAccess;

public class ModuleDTO(IReadOnlyList<string> referenceValues, IReadOnlyList<string> dependencyValues, string name)
{
	public string Name { get; } = name;
	public IReadOnlyList<string> DependencyValues { get; } = dependencyValues;
	public IReadOnlyList<string> ReferenceValues { get; } = referenceValues;
}