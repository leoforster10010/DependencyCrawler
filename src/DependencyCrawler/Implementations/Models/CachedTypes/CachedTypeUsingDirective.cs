using DependencyCrawler.Implementations.Data.Enum;
using DependencyCrawler.Implementations.Models.LinkedTypes;

namespace DependencyCrawler.Implementations.Models.CachedTypes;

public class CachedTypeUsingDirective : Entity
{
	public required string Name { get; init; }
	public required TypeUsingDirectiveState State { get; set; }
	public required Guid ReferencedNamespaceId { get; set; }
}