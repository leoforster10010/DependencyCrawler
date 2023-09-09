using DependencyCrawler.Data.Contracts.Entities.CachedTypes;
using DependencyCrawler.Data.Contracts.Enum;

namespace DependencyCrawler.Data.Contracts.Entities;

public class Cache
{
	public Guid Id { get; init; } = Guid.NewGuid();
	public CacheState State { get; set; } = CacheState.Active;
	public required IList<CachedProject> CachedProjects { get; set; }
	public DateTime Timestamp { get; init; } = DateTime.Now;
	public string? Name { get; init; }
}