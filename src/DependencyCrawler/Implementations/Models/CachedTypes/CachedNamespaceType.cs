namespace DependencyCrawler.Implementations.Models.CachedTypes;

public class CachedNamespaceType
{
	public required string Name { get; init; }
	public IList<CachedTypeUsingDirective> UsingDirectives { get; set; } = new List<CachedTypeUsingDirective>();
}