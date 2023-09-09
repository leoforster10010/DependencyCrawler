using DependencyCrawler.Data.Contracts.Entities;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

public interface ICacheManager
{
	IReadOnlyList<Cache> Caches { get; }
	Cache? ActiveCache { get; }
	void LoadCaches();
	void ActivateCache(Guid id);
	void DeleteCache(Guid id);
	void SaveAsCurrentCache();
	void SaveAsExistingCache(Guid id);
	void SaveAsNewCache(string? name = null);
	void SaveAllCaches();
	void ReloadCaches();
}