namespace DependencyCrawler.Implementations.Models.CachedTypes;

public class CachedProjectNamespace
{
	public Guid Id { get; } = Guid.NewGuid();
	public required string Name { get; init; }
	public IList<CachedNamespaceType> NamespaceTypes { get; set; } = new List<CachedNamespaceType>();
}