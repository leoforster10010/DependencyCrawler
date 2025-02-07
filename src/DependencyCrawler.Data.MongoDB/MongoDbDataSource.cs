using DependencyCrawler.DataCore.DataAccess;

namespace DependencyCrawler.Data.MongoDB;

public class MongoDbDataSource : IDataSource
{
	public Guid Id { get; } = Guid.NewGuid();

	public void Save()
	{
		throw new NotImplementedException();
	}

	public void Load()
	{
		throw new NotImplementedException();
	}
}