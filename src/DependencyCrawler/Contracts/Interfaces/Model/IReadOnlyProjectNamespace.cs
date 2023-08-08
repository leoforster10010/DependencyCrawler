namespace DependencyCrawler.Contracts.Interfaces.Model;

public interface IReadOnlyProjectNamespace
{
	public string NameReadOnly { get; }
	public IReadOnlyProject ParentProjectReadOnly { get; }
	public IReadOnlyDictionary<string, IReadOnlyNamespaceType> NamespaceTypesReadOnly { get; }
	public IReadOnlyDictionary<string, IReadOnlyNamespaceType> UsingTypesReadOnly { get; }
	public IReadOnlyDictionary<string, IReadOnlyTypeUsingDirective> TypeUsingDirectivesReadOnly { get; }
}