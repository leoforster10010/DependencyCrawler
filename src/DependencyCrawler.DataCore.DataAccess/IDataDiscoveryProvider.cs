namespace DependencyCrawler.DataCore.DataAccess;

public interface IDataDiscoveryProvider
{
	IReadOnlyDictionary<Guid, IDataDiscovery> DataDiscoveries { get; }
}