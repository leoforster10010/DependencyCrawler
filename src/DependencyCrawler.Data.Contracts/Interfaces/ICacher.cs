using DependencyCrawler.Data.Contracts.Entities;

namespace DependencyCrawler.Data.Contracts.Interfaces;

public interface ICacher
{
	void SaveCaches(IEnumerable<Cache> caches);
	IEnumerable<Cache> GetAvailableCaches();
}