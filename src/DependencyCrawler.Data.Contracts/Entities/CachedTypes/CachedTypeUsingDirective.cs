using DependencyCrawler.Data.Contracts.Enum;

namespace DependencyCrawler.Data.Contracts.Entities.CachedTypes;

public class CachedTypeUsingDirective : Entity
{
	public required string Name { get; init; }
	public required TypeUsingDirectiveState State { get; set; }
	public required Guid ReferencedNamespaceId { get; set; }
}