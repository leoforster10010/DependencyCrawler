namespace DependencyCrawler.DataCore;

internal interface IModuleRepository
{
	void Update(IValueModule valueModule);
	void Add(IValueModule valueModule);
}