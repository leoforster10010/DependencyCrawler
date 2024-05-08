using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataCore;

public interface IDataSource
{
	Guid Id { get; }
	public IDictionary<Guid, IValueDataCore> LoadCores();
	public void SaveCore(IValueDataCore valueCore);
}