using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Data.Contracts.Entities;

namespace DependencyCrawler.Implementations.Models.LinkedTypes;

internal class UnresolvedNamespace : Entity, IProjectNamespace
{
	public required string Name { get; init; }
	public IProject ParentProject { get; set; } = new UnresolvedProject();
	public IDictionary<Guid, INamespaceType> NamespaceTypes { get; set; } = new Dictionary<Guid, INamespaceType>();
	public IDictionary<Guid, INamespaceType> UsingTypes { get; set; } = new Dictionary<Guid, INamespaceType>();

	public IDictionary<Guid, ITypeUsingDirective> TypeUsingDirectives =>
		new Dictionary<Guid, ITypeUsingDirective>();

	public string NameReadOnly => Name;
	public IReadOnlyProject ParentProjectReadOnly => ParentProject;

	public IReadOnlyDictionary<Guid, IReadOnlyNamespaceType> NamespaceTypesReadOnly =>
		new Dictionary<Guid, IReadOnlyNamespaceType>();

	public IReadOnlyDictionary<Guid, IReadOnlyNamespaceType> UsingTypesReadOnly =>
		new Dictionary<Guid, IReadOnlyNamespaceType>();

	public IReadOnlyDictionary<Guid, IReadOnlyTypeUsingDirective> TypeUsingDirectivesReadOnly =>
		new Dictionary<Guid, IReadOnlyTypeUsingDirective>();
}