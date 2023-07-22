using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Contracts.Interfaces.Model;

public interface ITypeUsingDirective
{
	public string Name { get; }
	public TypeUsingDirectiveState State { get; set; }
	public IProjectNamespace ReferencedNamespace { get; set; }
	public INamespaceType ParentType { get; set; }
	public IProjectNamespace ParentNamespace { get; }
	public IProject ParentProject { get; }
}