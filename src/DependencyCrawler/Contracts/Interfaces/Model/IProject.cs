using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Contracts.Interfaces.Model;

internal interface IProject : IReadOnlyProject
{
	public string Name { get; init; }
	public ProjectType ProjectType { get; }
	public IDictionary<string, IReference> Dependencies { get; }
	public IDictionary<string, IReference> ReferencedBy { get; set; }
	public IDictionary<string, IProjectNamespace> Namespaces { get; set; }
	public IDictionary<string, INamespaceType> Types { get; }
	public IDictionary<string, ITypeUsingDirective> UsingDirectives { get; }
}