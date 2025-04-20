using DependencyCrawler.DataCore.DataAccess;

namespace DependencyCrawler.DataCore;

internal class DataDiscoveryProvider : IDataDiscoveryProvider
{
	private readonly Dictionary<Guid, IDataDiscovery> _dataDiscoveries = new();

	public DataDiscoveryProvider(IEnumerable<IDataDiscovery> dataDiscoveries)
	{
		foreach (var dataDiscovery in dataDiscoveries)
		{
			_dataDiscoveries.TryAdd(dataDiscovery.Id, dataDiscovery);
		}
	}

	public IReadOnlyDictionary<Guid, IDataDiscovery> DataDiscoveries => _dataDiscoveries.AsReadOnly();
}