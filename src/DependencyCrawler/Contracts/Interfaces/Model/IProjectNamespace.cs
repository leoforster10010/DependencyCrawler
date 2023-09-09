namespace DependencyCrawler.Contracts.Interfaces.Model;

internal interface IProjectNamespace : IReadOnlyProjectNamespace
{
	public string Name { get; init; }
	public IProject ParentProject { get; set; }
	public IDictionary<Guid, INamespaceType> NamespaceTypes { get; set; }
	public IDictionary<Guid, INamespaceType> UsingTypes { get; set; }
	public IDictionary<Guid, ITypeUsingDirective> TypeUsingDirectives { get; }
}