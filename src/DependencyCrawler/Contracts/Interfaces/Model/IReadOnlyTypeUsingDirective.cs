using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Contracts.Interfaces.Model;

public interface IReadOnlyTypeUsingDirective
{
	public string NameReadOnly { get; }
	public TypeUsingDirectiveState StateReadOnlyReadOnly { get; }
	public IReadOnlyProjectNamespace ReferencedNamespaceReadOnly { get; }
	public IReadOnlyNamespaceType ParentTypeReadOnly { get; }
	public IReadOnlyProjectNamespace ParentNamespaceReadOnly { get; }
	public IReadOnlyProject ParentProjectReadOnly { get; }
}