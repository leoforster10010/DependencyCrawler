namespace DependencyCrawler.DataCore;

internal class DataSourceProvider(IEnumerable<IDataSource> dataSources, IDataCoreProvider dependencyCrawlerCoreProvider)
{
	public IReadOnlyDictionary<Guid, IDataSource> DataSources => dataSources.ToDictionary(x => x.Id, y => y);

	public void LoadAll()
	{
		foreach (var dataSource in dataSources)
		{
			var loadedCores = dataSource.LoadCores();

			foreach (var dependencyCrawlerValueCore in loadedCores)
			{
				dependencyCrawlerCoreProvider.AddCore(dependencyCrawlerValueCore.Value);
			}
		}
	}
}