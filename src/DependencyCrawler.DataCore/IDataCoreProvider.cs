using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataCore;

internal interface IDataCoreProvider
{
	DataCore ActiveCore { get; set; }
	IReadOnlySet<IValueDataCore> ValueDataCores { get; }
	void AddCore(IValueDataCore valueCore, bool activate = false);
	void SaveTo(Guid dataSourceId);
}