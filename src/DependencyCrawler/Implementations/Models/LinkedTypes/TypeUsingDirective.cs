using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Implementations.Models.LinkedTypes;

public class TypeUsingDirective : ITypeUsingDirective
{
	public string Name => ReferencedNamespace.Name;
	public required TypeUsingDirectiveState State { get; set; }
	public required IProjectNamespace ReferencedNamespace { get; set; }
	public required INamespaceType ParentType { get; set; }
	public IProjectNamespace ParentNamespace => ParentType.ParentNamespace;
	public IProject ParentProject => ParentType.ParentProject;
}