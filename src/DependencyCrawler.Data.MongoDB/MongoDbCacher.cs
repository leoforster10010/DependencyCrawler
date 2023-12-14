using DependencyCrawler.Data.Contracts.Entities;
using DependencyCrawler.Data.Contracts.Interfaces;

namespace DependencyCrawler.Data.MongoDB;

public class MongoDbCacher : ICacher
{
	public void SaveCaches(IEnumerable<Cache> caches)
	{
		throw new NotImplementedException();
	}

	public IEnumerable<Cache> GetAvailableCaches()
	{
		throw new NotImplementedException();
	}
}