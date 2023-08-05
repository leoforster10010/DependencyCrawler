using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Implementations.Models.CachedTypes;

public class CachedTypeUsingDirective
{
	public required string Name { get; init; }
	public required TypeUsingDirectiveState State { get; set; }
	public required Guid ReferencedNamespaceId { get; set; }
}