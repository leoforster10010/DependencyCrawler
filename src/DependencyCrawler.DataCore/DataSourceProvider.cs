namespace DependencyCrawler.DataCore;

public interface IDataSourceProvider
{
	IReadOnlyDictionary<Guid, IDataSource> DataSources { get; }
}

public class DataSourceProvider : IDataSourceProvider
{
	private readonly IDictionary<Guid, IDataSource> _dataSources;

	public DataSourceProvider(IEnumerable<IDataSource> dataSources)
	{
		_dataSources = dataSources.ToDictionary(k => k.Id, v => v);
	}

	public IReadOnlyDictionary<Guid, IDataSource> DataSources => _dataSources.AsReadOnly();
}