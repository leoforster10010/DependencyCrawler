using DependencyCrawler.Contracts.Interfaces.Model;

namespace DependencyCrawler.Implementations.Models;

public class NamespaceType : INamespaceType
{
	public required string Name { get; init; }
	public string FullName => $"{ParentNamespace.Name}.{Name}";
	public required IProjectNamespace ParentNamespace { get; set; }
	public IProject ParentProject => ParentNamespace.ParentProject;

	public IDictionary<string, ITypeUsingDirective> UsingDirectives { get; set; } =
		new Dictionary<string, ITypeUsingDirective>();
}