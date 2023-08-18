using DependencyCrawler.Implementations.Models.LinkedTypes;

namespace DependencyCrawler.Contracts.Interfaces.Model;

public interface IReadOnlyProjectNamespace : IEntity
{
	public string NameReadOnly { get; }
	public IReadOnlyProject ParentProjectReadOnly { get; }
	public IReadOnlyDictionary<Guid, IReadOnlyNamespaceType> NamespaceTypesReadOnly { get; }
	public IReadOnlyDictionary<Guid, IReadOnlyNamespaceType> UsingTypesReadOnly { get; }
	public IReadOnlyDictionary<Guid, IReadOnlyTypeUsingDirective> TypeUsingDirectivesReadOnly { get; }
}