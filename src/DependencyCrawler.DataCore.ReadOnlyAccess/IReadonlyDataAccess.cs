namespace DependencyCrawler.DataCore.ReadOnlyAccess;

public interface IReadOnlyDataAccess
{
	IReadOnlyDataCore DataReadOnly { get; }
}