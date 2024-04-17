namespace DependencyCrawler.DataCore;

internal interface IDependencyCrawlerCoreProvider
{
	DependencyCrawlerCore ActiveCore { get; set; }
	void AddCore(IDependencyCrawlerValueCore valueCore, bool activate = false);
	void RemoveCore(Guid id);
}