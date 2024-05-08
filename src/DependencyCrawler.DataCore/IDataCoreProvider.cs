using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataCore;

internal interface IDataCoreProvider
{
	DataCore ActiveCore { get; set; }
	void AddCore(IValueDataCore valueCore, bool activate = false);
	void RemoveCore(Guid id);
}