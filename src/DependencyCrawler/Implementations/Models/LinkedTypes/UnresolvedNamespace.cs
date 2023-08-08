using DependencyCrawler.Contracts.Interfaces.Model;

namespace DependencyCrawler.Implementations.Models.LinkedTypes;

internal class UnresolvedNamespace : IProjectNamespace
{
	public required string Name { get; init; }
	public IProject ParentProject { get; set; } = new UnresolvedProject();
	public IDictionary<string, INamespaceType> NamespaceTypes { get; set; } = new Dictionary<string, INamespaceType>();
	public IDictionary<string, INamespaceType> UsingTypes { get; set; } = new Dictionary<string, INamespaceType>();

	public IDictionary<string, ITypeUsingDirective> TypeUsingDirectives =>
		new Dictionary<string, ITypeUsingDirective>();

	public string NameReadOnly => Name;
	public IReadOnlyProject ParentProjectReadOnly => ParentProject;

	public IReadOnlyDictionary<string, IReadOnlyNamespaceType> NamespaceTypesReadOnly =>
		new Dictionary<string, IReadOnlyNamespaceType>();

	public IReadOnlyDictionary<string, IReadOnlyNamespaceType> UsingTypesReadOnly =>
		new Dictionary<string, IReadOnlyNamespaceType>();

	public IReadOnlyDictionary<string, IReadOnlyTypeUsingDirective> TypeUsingDirectivesReadOnly =>
		new Dictionary<string, IReadOnlyTypeUsingDirective>();
}