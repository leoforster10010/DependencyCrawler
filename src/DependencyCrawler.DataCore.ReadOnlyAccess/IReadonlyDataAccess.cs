namespace DependencyCrawler.DataCore.ReadOnlyAccess;

public interface IReadonlyDataAccess
{
	IReadonlyDataCore ReadonlyCore { get; }
}