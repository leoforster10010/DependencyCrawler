using System.Collections.Concurrent;
using DependencyCrawler.DataCore.DataAccess;

namespace DependencyCrawler.DataCore;

internal class DataSourceProvider : IDataSourceProvider
{
	private readonly IDictionary<Guid, IDataSource> _dataSources = new ConcurrentDictionary<Guid, IDataSource>();

	public DataSourceProvider(IEnumerable<IDataSource> dataSources)
	{
		foreach (var dataSource in dataSources)
		{
			_dataSources.TryAdd(dataSource.Id, dataSource);
		}
	}

	public IReadOnlyDictionary<Guid, IDataSource> DataSources => _dataSources.AsReadOnly();
}