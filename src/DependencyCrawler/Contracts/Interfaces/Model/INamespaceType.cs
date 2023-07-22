namespace DependencyCrawler.Contracts.Interfaces.Model;

public interface INamespaceType
{
	public string Name { get; init; }
	public string FullName { get; }
	public IProjectNamespace ParentNamespace { get; set; }
	public IProject ParentProject { get; }
	public IDictionary<string, ITypeUsingDirective> UsingDirectives { get; set; }
}