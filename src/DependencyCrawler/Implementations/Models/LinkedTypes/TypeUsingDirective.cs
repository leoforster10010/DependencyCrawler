using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Data.Contracts.Entities;
using DependencyCrawler.Data.Contracts.Enum;

namespace DependencyCrawler.Implementations.Models.LinkedTypes;

internal class TypeUsingDirective : Entity, ITypeUsingDirective
{
	public string Name => ReferencedNamespace.Name;
	public required TypeUsingDirectiveState State { get; set; }
	public required IProjectNamespace ReferencedNamespace { get; set; }
	public required INamespaceType ParentType { get; set; }
	public IProjectNamespace ParentNamespace => ParentType.ParentNamespace;
	public IProject ParentProject => ParentType.ParentProject;
	public string NameReadOnly => Name;
	public TypeUsingDirectiveState StateReadOnly => State;
	public IReadOnlyProjectNamespace ReferencedNamespaceReadOnly => ReferencedNamespace;
	public IReadOnlyNamespaceType ParentTypeReadOnly => ParentType;
	public IReadOnlyProjectNamespace ParentNamespaceReadOnly => ParentNamespace;
	public IReadOnlyProject ParentProjectReadOnly => ParentProject;
}