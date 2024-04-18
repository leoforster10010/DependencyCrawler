using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataCore.DataAccess;

public interface IModuleRepository
{
	void Update(IValueModule valueModule);
	void Add(IValueModule valueModule);
}