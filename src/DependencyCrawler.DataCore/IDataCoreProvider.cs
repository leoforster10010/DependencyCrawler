namespace DependencyCrawler.DataCore;

internal interface IDataCoreProvider
{
	IReadOnlyDictionary<Guid, IDataCore> DataCores { get; }
	IDataCore ActiveCore { get; }
	IDataCore CreateDataCore();
}