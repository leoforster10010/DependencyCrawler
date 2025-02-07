namespace DependencyCrawler.DataCore.DataAccess;

public interface IDataSource
{
	Guid Id { get; }
	void Save();
	void Load();
}