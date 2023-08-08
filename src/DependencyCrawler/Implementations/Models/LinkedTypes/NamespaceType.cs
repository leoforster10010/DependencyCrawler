using DependencyCrawler.Contracts.Interfaces.Model;

namespace DependencyCrawler.Implementations.Models.LinkedTypes;

internal class NamespaceType : INamespaceType
{
	public required string Name { get; init; }
	public string FullName => $"{ParentNamespace.Name}.{Name}";
	public required IProjectNamespace ParentNamespace { get; set; }
	public IProject ParentProject => ParentNamespace.ParentProject;

	public IDictionary<string, ITypeUsingDirective> UsingDirectives { get; set; } =
		new Dictionary<string, ITypeUsingDirective>();

	public string NameReadOnly => Name;
	public string FullNameReadOnly => FullName;
	public IReadOnlyProjectNamespace ParentNamespaceReadOnly => ParentNamespace;
	public IReadOnlyProject ParentProjectReadOnly => ParentProject;

	public IReadOnlyDictionary<string, IReadOnlyTypeUsingDirective> UsingDirectivesReadOnly =>
		UsingDirectives.ToDictionary(x => x.Key, x => x.Value as IReadOnlyTypeUsingDirective);
}