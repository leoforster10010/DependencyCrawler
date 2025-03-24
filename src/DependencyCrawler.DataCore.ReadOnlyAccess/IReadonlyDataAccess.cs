namespace DependencyCrawler.DataCore.ReadOnlyAccess;

public interface IReadOnlyDataAccess
{
	IReadOnlyDataCore ActiveCoreReadOnly { get; }
	event Action? DataCoreActivated;
}