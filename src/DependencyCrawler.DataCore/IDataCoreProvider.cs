using DependencyCrawler.DataCore.ReadOnlyAccess;
using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataCore;

public interface IDataCoreProvider : IReadOnlyDataAccess, IValueDataAccess
{
	IReadOnlyDictionary<Guid, IDataCore> DataCores { get; }
	IDataCore ActiveCore { get; }
	IDataCore CreateDataCore();
}