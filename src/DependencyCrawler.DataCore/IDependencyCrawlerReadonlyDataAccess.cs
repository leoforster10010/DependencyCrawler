namespace DependencyCrawler.DataCore;

internal interface IDependencyCrawlerReadonlyDataAccess
{
	IDependencyCrawlerReadonlyCore ReadonlyCore { get; }
}