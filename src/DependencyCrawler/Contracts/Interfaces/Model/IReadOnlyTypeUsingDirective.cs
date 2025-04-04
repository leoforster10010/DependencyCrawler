using DependencyCrawler.Data.Contracts.Enum;
using DependencyCrawler.Data.Contracts.Interfaces;

namespace DependencyCrawler.Contracts.Interfaces.Model;

public interface IReadOnlyTypeUsingDirective : IEntity
{
	public string NameReadOnly { get; }
	public TypeUsingDirectiveState StateReadOnly { get; }
	public IReadOnlyProjectNamespace ReferencedNamespaceReadOnly { get; }
	public IReadOnlyNamespaceType ParentTypeReadOnly { get; }
	public IReadOnlyProjectNamespace ParentNamespaceReadOnly { get; }
	public IReadOnlyProject ParentProjectReadOnly { get; }
}