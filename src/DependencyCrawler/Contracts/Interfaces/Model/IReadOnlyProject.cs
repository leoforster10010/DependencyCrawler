using DependencyCrawler.Data.Contracts.Enum;
using DependencyCrawler.Data.Contracts.Interfaces;

namespace DependencyCrawler.Contracts.Interfaces.Model;

public interface IReadOnlyProject : IEntity
{
	public string NameReadOnly { get; }
	public ProjectType ProjectTypeReadOnly { get; }
	public int ReferenceLayerReadOnly { get; }
	public int DependencyLayerReadOnly { get; }
	public int DependencyLayerInternalReadOnly { get; }
	public IReadOnlyDictionary<Guid, IReadOnlyReference> DependenciesReadOnly { get; }
	public IReadOnlyDictionary<Guid, IReadOnlyReference> ReferencesReadOnly { get; }
	public IReadOnlyDictionary<Guid, IReadOnlyProjectNamespace> NamespacesReadOnly { get; }
	public IReadOnlyDictionary<Guid, IReadOnlyNamespaceType> TypesReadOnly { get; }
	public IReadOnlyDictionary<Guid, IReadOnlyTypeUsingDirective> UsingDirectivesReadOnly { get; }
}