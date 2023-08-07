using DependencyCrawler.Implementations.Data.Enum;
using DependencyCrawler.Implementations.Models.CachedTypes;

namespace DependencyCrawler.Implementations.Models;

public class Cache
{
	public Guid Id { get; init; } = Guid.NewGuid();
	public CacheState State { get; set; } = CacheState.Active;
	public required IList<CachedProject> CachedProjects { get; set; }
	public DateTime Timestamp { get; init; } = DateTime.Now;
	public string? Name { get; init; }
}