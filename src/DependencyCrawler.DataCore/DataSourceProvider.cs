using DependencyCrawler.DataCore.DataAccess;

namespace DependencyCrawler.DataCore;

internal class DataSourceProvider : IDataSourceProvider
{
	private readonly Dictionary<Guid, IDataSource> _dataSources = new();

	public DataSourceProvider(IEnumerable<IDataSource> dataSources)
	{
		foreach (var dataSource in dataSources)
		{
			_dataSources.TryAdd(dataSource.Id, dataSource);
		}
	}

	public IReadOnlyDictionary<Guid, IDataSource> DataSources => _dataSources.AsReadOnly();
}