using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Contracts.Interfaces.Model;

public interface IReadOnlyProject
{
	public string NameReadOnly { get; }
	public ProjectType ProjectTypeReadOnly { get; }
	public IReadOnlyDictionary<string, IReadOnlyReference> DependenciesReadOnly { get; }
	public IReadOnlyDictionary<string, IReadOnlyReference> ReferencedByReadOnly { get; }
	public IReadOnlyDictionary<string, IReadOnlyProjectNamespace> NamespacesReadOnly { get; }
	public IReadOnlyDictionary<string, IReadOnlyNamespaceType> TypesReadOnly { get; }
	public IReadOnlyDictionary<string, IReadOnlyTypeUsingDirective> UsingDirectivesReadOnly { get; }
}