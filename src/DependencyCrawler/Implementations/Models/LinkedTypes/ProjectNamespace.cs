using DependencyCrawler.Contracts.Interfaces.Model;

namespace DependencyCrawler.Implementations.Models.LinkedTypes;

public class ProjectNamespace : IProjectNamespace
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
}