namespace DependencyCrawler.Data.Contracts.Entities.CachedTypes;

public class CachedProjectNamespace : Entity
{
	public required string Name { get; init; }
	public IList<CachedNamespaceType> NamespaceTypes { get; set; } = new List<CachedNamespaceType>();
}