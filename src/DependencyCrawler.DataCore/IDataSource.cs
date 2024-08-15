using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataCore;

public interface IDataSource
{
	Guid Id { get; }
	public HashSet<IValueDataCore> LoadCores();
	public void SaveCores(HashSet<IValueDataCore> dataCores);
}