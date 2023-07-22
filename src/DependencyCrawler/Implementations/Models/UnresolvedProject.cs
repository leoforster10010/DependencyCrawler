using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Implementations.Models;

public class UnresolvedProject : IProject
{
	public string Name { get; init; } = string.Empty;
	public ProjectType ProjectType => ProjectType.Unresolved;
	public IDictionary<string, IReference> Dependencies => new Dictionary<string, IReference>();
	public IDictionary<string, IReference> ReferencedBy { get; set; } = new Dictionary<string, IReference>();

	public IDictionary<string, IProjectNamespace> Namespaces { get; set; } =
		new Dictionary<string, IProjectNamespace>();

	public IDictionary<string, INamespaceType> Types => new Dictionary<string, INamespaceType>();
	public IDictionary<string, ITypeUsingDirective> UsingDirectives => new Dictionary<string, ITypeUsingDirective>();
}