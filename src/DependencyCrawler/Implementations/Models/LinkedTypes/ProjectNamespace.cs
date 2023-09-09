using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Data.Contracts.Entities;

namespace DependencyCrawler.Implementations.Models.LinkedTypes;

internal class ProjectNamespace : Entity, IProjectNamespace
{
	public required string Name { get; init; }
	public required IProject ParentProject { get; set; }
	public IDictionary<Guid, INamespaceType> NamespaceTypes { get; set; } = new Dictionary<Guid, INamespaceType>();

	public IDictionary<Guid, INamespaceType> UsingTypes { get; set; } =
		new Dictionary<Guid, INamespaceType>();

	public IDictionary<Guid, ITypeUsingDirective> TypeUsingDirectives
	{
		get
		{
			var namespaceUsingDirectives = new Dictionary<Guid, ITypeUsingDirective>();
			var usingDirectives = NamespaceTypes.Values.SelectMany(x => x.UsingDirectives.Values);
			foreach (var usingDirective in usingDirectives)
			{
				namespaceUsingDirectives.TryAdd(usingDirective.Id, usingDirective);
			}

			return namespaceUsingDirectives;
		}
	}

	public string NameReadOnly => Name;
	public IReadOnlyProject ParentProjectReadOnly => ParentProject;

	public IReadOnlyDictionary<Guid, IReadOnlyNamespaceType> NamespaceTypesReadOnly =>
		NamespaceTypes.ToDictionary(x => x.Key, x => x.Value as IReadOnlyNamespaceType);

	public IReadOnlyDictionary<Guid, IReadOnlyNamespaceType> UsingTypesReadOnly =>
		UsingTypes.ToDictionary(x => x.Key, x => x.Value as IReadOnlyNamespaceType);

	public IReadOnlyDictionary<Guid, IReadOnlyTypeUsingDirective> TypeUsingDirectivesReadOnly =>
		TypeUsingDirectives.ToDictionary(x => x.Key, x => x.Value as IReadOnlyTypeUsingDirective);
}