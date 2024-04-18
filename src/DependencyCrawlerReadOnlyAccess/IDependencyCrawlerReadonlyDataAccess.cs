namespace DependencyCrawler.DataCore.ReadOnlyAccess;

public interface IDependencyCrawlerReadonlyDataAccess
{
	IDependencyCrawlerReadonlyCore ReadonlyCore { get; }
}