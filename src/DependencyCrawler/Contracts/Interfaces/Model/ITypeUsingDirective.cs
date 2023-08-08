using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Contracts.Interfaces.Model;

internal interface ITypeUsingDirective : IReadOnlyTypeUsingDirective
{
	public string Name { get; }
	public TypeUsingDirectiveState State { get; set; }
	public IProjectNamespace ReferencedNamespace { get; set; }
	public INamespaceType ParentType { get; set; }
	public IProjectNamespace ParentNamespace { get; }
	public IProject ParentProject { get; }
}