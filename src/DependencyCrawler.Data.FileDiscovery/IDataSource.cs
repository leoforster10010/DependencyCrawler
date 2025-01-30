using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.Data.FileDiscovery;

public interface IDataSource
{
	public HashSet<IValueDataCore> LoadCores();
	public void SaveCores(HashSet<IValueDataCore> dataCores);
}