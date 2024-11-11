namespace DependencyCrawler.DataCore.ValueAccess;

public class ValueModule(IReadOnlyList<string> referenceValues, IReadOnlyList<string> dependencyValues, string name) : IValueModule
{
    public string Name { get; } = name;
    public IReadOnlyList<string> DependencyValues { get; } = dependencyValues;
    public IReadOnlyList<string> ReferenceValues { get; } = referenceValues;
}