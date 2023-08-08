using DependencyCrawler.Contracts.Interfaces.Model;

namespace DependencyCrawler.Implementations.Models.LinkedTypes;

internal class ProjectNamespace : IProjectNamespace
{
	public required string Name { get; init; }
	public required IProject ParentProject { get; set; }
	public IDictionary<string, INamespaceType> NamespaceTypes { get; set; } = new Dictionary<string, INamespaceType>();

	public IDictionary<string, INamespaceType> UsingTypes { get; set; } =
		new Dictionary<string, INamespaceType>();

	public IDictionary<string, ITypeUsingDirective> TypeUsingDirectives
	{
		get
		{
			var namespaceUsingDirectives = new Dictionary<string, ITypeUsingDirective>();
			var usingDirectives = NamespaceTypes.Values.SelectMany(x => x.UsingDirectives.Values);
			foreach (var usingDirective in usingDirectives)
			{
				namespaceUsingDirectives.TryAdd(usingDirective.Name, usingDirective);
			}

			return namespaceUsingDirectives;
		}
	}

	public string NameReadOnly => Name;
	public IReadOnlyProject ParentProjectReadOnly => ParentProject;

	public IReadOnlyDictionary<string, IReadOnlyNamespaceType> NamespaceTypesReadOnly =>
		NamespaceTypes.ToDictionary(x => x.Key, x => x.Value as IReadOnlyNamespaceType);

	public IReadOnlyDictionary<string, IReadOnlyNamespaceType> UsingTypesReadOnly =>
		UsingTypes.ToDictionary(x => x.Key, x => x.Value as IReadOnlyNamespaceType);

	public IReadOnlyDictionary<string, IReadOnlyTypeUsingDirective> TypeUsingDirectivesReadOnly =>
		TypeUsingDirectives.ToDictionary(x => x.Key, x => x.Value as IReadOnlyTypeUsingDirective);
}