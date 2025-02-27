namespace DependencyCrawler.DataCore.DataAccess;

public interface IDataSource
{
	Guid Id { get; }
	Task Save();
	Task Load();
}