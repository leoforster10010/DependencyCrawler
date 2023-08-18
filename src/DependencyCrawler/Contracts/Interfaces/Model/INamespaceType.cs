namespace DependencyCrawler.Contracts.Interfaces.Model;

internal interface INamespaceType : IReadOnlyNamespaceType
{
	public string Name { get; init; }
	public string FullName { get; }
	public IProjectNamespace ParentNamespace { get; set; }
	public IProject ParentProject { get; }
	public IDictionary<Guid, ITypeUsingDirective> UsingDirectives { get; set; }
}