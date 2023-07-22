namespace DependencyCrawler.Contracts.Interfaces.Model;

public interface IProjectNamespace
{
	public string Name { get; init; }
	public IProject ParentProject { get; set; }
	public IDictionary<string, INamespaceType> NamespaceTypes { get; set; }
	public IDictionary<string, INamespaceType> UsingTypes { get; set; }
	public IDictionary<string, ITypeUsingDirective> TypeUsingDirectives { get; }
}