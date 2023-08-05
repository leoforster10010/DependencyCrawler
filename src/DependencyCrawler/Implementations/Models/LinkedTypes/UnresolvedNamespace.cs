using DependencyCrawler.Contracts.Interfaces.Model;

namespace DependencyCrawler.Implementations.Models.LinkedTypes;

public class UnresolvedNamespace : IProjectNamespace
{
	public required string Name { get; init; }
	public IProject ParentProject { get; set; } = new UnresolvedProject();
	public IDictionary<string, INamespaceType> NamespaceTypes { get; set; } = new Dictionary<string, INamespaceType>();
	public IDictionary<string, INamespaceType> UsingTypes { get; set; } = new Dictionary<string, INamespaceType>();

	public IDictionary<string, ITypeUsingDirective> TypeUsingDirectives =>
		new Dictionary<string, ITypeUsingDirective>();
}