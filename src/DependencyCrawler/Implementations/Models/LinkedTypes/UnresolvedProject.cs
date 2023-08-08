using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Implementations.Models.LinkedTypes;

internal class UnresolvedProject : IProject
{
	public string Name { get; init; } = string.Empty;
	public ProjectType ProjectType => ProjectType.Unresolved;
	public IDictionary<string, IReference> Dependencies => new Dictionary<string, IReference>();
	public IDictionary<string, IReference> ReferencedBy { get; set; } = new Dictionary<string, IReference>();

	public IDictionary<string, IProjectNamespace> Namespaces { get; set; } =
		new Dictionary<string, IProjectNamespace>();

	public IDictionary<string, INamespaceType> Types => new Dictionary<string, INamespaceType>();
	public IDictionary<string, ITypeUsingDirective> UsingDirectives => new Dictionary<string, ITypeUsingDirective>();
	public string NameReadOnly => Name;
	public ProjectType ProjectTypeReadOnly => ProjectType;

	public IReadOnlyDictionary<string, IReadOnlyReference> DependenciesReadOnly =>
		new Dictionary<string, IReadOnlyReference>();

	public IReadOnlyDictionary<string, IReadOnlyReference> ReferencedByReadOnly =>
		new Dictionary<string, IReadOnlyReference>();

	public IReadOnlyDictionary<string, IReadOnlyProjectNamespace> NamespacesReadOnly =>
		new Dictionary<string, IReadOnlyProjectNamespace>();

	public IReadOnlyDictionary<string, IReadOnlyNamespaceType> TypesReadOnly =>
		new Dictionary<string, IReadOnlyNamespaceType>();

	public IReadOnlyDictionary<string, IReadOnlyTypeUsingDirective> UsingDirectivesReadOnly =>
		new Dictionary<string, IReadOnlyTypeUsingDirective>();
}