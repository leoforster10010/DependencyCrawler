using DependencyCrawler.Implementations.Models;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

public interface ICacher
{
	void SaveCaches(IEnumerable<Cache> caches);
	IEnumerable<Cache> GetAvailableCaches();
}