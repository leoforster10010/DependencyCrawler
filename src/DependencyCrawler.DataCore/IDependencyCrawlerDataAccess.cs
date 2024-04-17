namespace DependencyCrawler.DataCore;

internal interface IDependencyCrawlerDataAccess
{
	IDependencyCrawlerCore Core { get; }
}