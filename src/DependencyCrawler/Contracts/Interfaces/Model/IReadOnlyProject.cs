using DependencyCrawler.Implementations.Data.Enum;
using DependencyCrawler.Implementations.Models.LinkedTypes;

namespace DependencyCrawler.Contracts.Interfaces.Model;

public interface IReadOnlyProject : IEntity
{
	public string NameReadOnly { get; }
	public ProjectType ProjectTypeReadOnly { get; }
	public IReadOnlyDictionary<Guid, IReadOnlyReference> DependenciesReadOnly { get; }
	public IReadOnlyDictionary<Guid, IReadOnlyReference> ReferencedByReadOnly { get; }
	public IReadOnlyDictionary<Guid, IReadOnlyProjectNamespace> NamespacesReadOnly { get; }
	public IReadOnlyDictionary<Guid, IReadOnlyNamespaceType> TypesReadOnly { get; }
	public IReadOnlyDictionary<Guid, IReadOnlyTypeUsingDirective> UsingDirectivesReadOnly { get; }
}