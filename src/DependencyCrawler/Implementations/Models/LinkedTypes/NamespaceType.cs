using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Data.Contracts.Entities;
using DependencyCrawler.Data.Contracts.Enum;

namespace DependencyCrawler.Implementations.Models.LinkedTypes;

internal class NamespaceType : Entity, INamespaceType
{
	public required string Name { get; init; }
	public string FullName => $"{ParentNamespace.Name}.{Name}";
	public required FileType FileType { get; init; }
	public required IProjectNamespace ParentNamespace { get; set; }
	public IProject ParentProject => ParentNamespace.ParentProject;

	public IDictionary<Guid, ITypeUsingDirective> UsingDirectives { get; set; } =
		new Dictionary<Guid, ITypeUsingDirective>();

	public string NameReadOnly => Name;
	public string FullNameReadOnly => FullName;
	public FileType FileTypeReadOnly => FileType;
	public IReadOnlyProjectNamespace ParentNamespaceReadOnly => ParentNamespace;
	public IReadOnlyProject ParentProjectReadOnly => ParentProject;

	public IReadOnlyDictionary<Guid, IReadOnlyTypeUsingDirective> UsingDirectivesReadOnly =>
		UsingDirectives.ToDictionary(x => x.Key, x => x.Value as IReadOnlyTypeUsingDirective);
}