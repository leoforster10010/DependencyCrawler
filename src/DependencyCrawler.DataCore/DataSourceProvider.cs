using System.Collections.Concurrent;

namespace DependencyCrawler.DataCore;

internal class DataSourceProvider : IDataSourceProvider
{
	private readonly IDictionary<Guid, IDataSource> _dataSources = new ConcurrentDictionary<Guid, IDataSource>();

	public DataSourceProvider(IEnumerable<IDataSource> dataSources)
	{
		foreach (var dataSource in dataSources)
		{
			if (_dataSources.TryAdd(dataSource.Id, dataSource))
			{
				dataSource.Load();
			}
		}
	}

	public IReadOnlyDictionary<Guid, IDataSource> DataSources => _dataSources.AsReadOnly();
}