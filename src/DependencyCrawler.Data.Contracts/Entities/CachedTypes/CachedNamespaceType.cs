using DependencyCrawler.Data.Contracts.Enum;

namespace DependencyCrawler.Data.Contracts.Entities.CachedTypes;

public class CachedNamespaceType : Entity
{
	public required string Name { get; init; }
	public required FileType FileType { get; init; }
	public IList<CachedTypeUsingDirective> UsingDirectives { get; set; } = new List<CachedTypeUsingDirective>();
}