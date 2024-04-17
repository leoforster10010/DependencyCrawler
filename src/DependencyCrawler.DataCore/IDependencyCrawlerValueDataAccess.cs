namespace DependencyCrawler.DataCore;

internal interface IDependencyCrawlerValueDataAccess
{
	IDependencyCrawlerValueCore ValueCore { get; }
}