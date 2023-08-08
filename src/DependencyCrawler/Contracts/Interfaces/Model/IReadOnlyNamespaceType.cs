namespace DependencyCrawler.Contracts.Interfaces.Model;

public interface IReadOnlyNamespaceType
{
	public string NameReadOnly { get; }
	public string FullNameReadOnly { get; }
	public IReadOnlyProjectNamespace ParentNamespaceReadOnly { get; }
	public IReadOnlyProject ParentProjectReadOnly { get; }
	public IReadOnlyDictionary<string, IReadOnlyTypeUsingDirective> UsingDirectivesReadOnly { get; }
}